#include "ObjectDetector.hpp"
#include "../../Settings.h"

using namespace std;
using namespace cv;
using namespace robotmapping;

ObjectDetector::ObjectDetector(const ObjectDetector& copy) noexcept : ShapeDetectorBase(copy._receiver), Runnable()
	, _sampleName(copy._sampleName),
	_trainKeypoints(copy._trainKeypoints.begin(), copy._trainKeypoints.end())
{
	_trainDescriptor = copy._trainDescriptor.clone();
	_trainSample = copy._trainSample.clone();
	
	_orb = ORB::create();
    _matcher = new cv::BFMatcher(cv::NORM_HAMMING, true);
}

ObjectDetector::ObjectDetector(IShapeDetectionEvents* receiver, const string& sampleName) : ShapeDetectorBase(receiver), Runnable()
	, _sampleName(sampleName)
{
	if(strcmp(sampleName.c_str(), "") == 0)
		throw invalid_argument("[ObjectDetector] Empty sampleName in config.yml is invalid.");

	string sampleFilePath = settings->getFilePath() + "samples/" + sampleName + ".yml";

    FileStorage fs2(sampleFilePath, FileStorage::READ);

	if(!fs2.isOpened())
		throw invalid_argument("[ObjectDetector] sample '" + sampleName + "' provided in config.yml is inaccessible at " + sampleFilePath + ".");

    FileNode descriptorNode = fs2["Descriptors"];
    FileNode keypointsNode = fs2["Keypoints"];
    FileNode sampleNode = fs2["Sample"];

    if(descriptorNode.isNone() || keypointsNode.isNone() || sampleNode.isNone()) //We couldn't find the descriptors in the file
        throw runtime_error("[ObjectDetector] Sample '" + sampleName + "' is not complete!");

    read(descriptorNode, _trainDescriptor);
    read(keypointsNode, _trainKeypoints);
    read(sampleNode, _trainSample);

    fs2.release();

    cout << "[ObjectDetector] Read sample '" + sampleName + "' from file." << endl;

	_orb = ORB::create();
    _matcher = new cv::BFMatcher(cv::NORM_HAMMING, true);
}

ObjectDetector::~ObjectDetector()
{
	//TODO: Check if EVERYTHING is deleted
	Stop();
}


vector<RotatedRect> ObjectDetector::detectMovement(const Mat& bufferFrame, const Mat& currentFrame)
{
	vector<RotatedRect> movingBlobs;

	Mat grayBuffer, grayCurrent, filteredImage;
	cvtColor(bufferFrame, grayBuffer, CV_BGR2GRAY);
	cvtColor(currentFrame, grayCurrent, CV_BGR2GRAY);

	absdiff(grayBuffer, grayCurrent, filteredImage);
	threshold(filteredImage, filteredImage, 10, 255, CV_THRESH_BINARY);

	medianBlur(filteredImage, filteredImage, 5);

	vector<vector<Point>> contours;

	if(countNonZero(filteredImage) < 1) //Check if there is something moving, otherwise boundingRect will throw an exception
		return movingBlobs; //Return empty vector<RotatedRect>


	Mat closedBlobs;
	cv::Mat structuringElement = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(40, 40));
	cv::morphologyEx(filteredImage, closedBlobs, cv::MORPH_CLOSE, structuringElement); //TODO: Check what happens when multiple blobs are found

	findContours(closedBlobs, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);
	//findContours(filteredImage, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);


	//Removes all contours that are smaller than MINIMUM_BLOB_AREA pixels in area.
	//Converts them to a RotatedRect and puts them in 'movingBlobs'
	for(const auto& contour : contours)
	{
		RotatedRect rect = minAreaRect(contour);

		if(rect.size.area() > MINIMUM_BLOB_AREA)
		{
			movingBlobs.push_back(rect);
		}
	}

	return movingBlobs;
}

void ObjectDetector::run()
{	
	if(_bufferFrameMoveHelper.HasFrame() && _currentFrameMoveHelper.HasFrame())
	{
		Mat bufferFrame = _bufferFrameMoveHelper.AcceptFrame();
		Mat currentFrame = _currentFrameMoveHelper.AcceptFrame();

		_receiver->SignalNewFrame(*this);



		Mat queryDescriptor;
		vector<KeyPoint> keypoints(200);

		_orb->detect(currentFrame, keypoints);
		_orb->compute(currentFrame, keypoints, queryDescriptor);

		vector<DMatch> matches;
		_matcher->match(queryDescriptor, _trainDescriptor, matches); //TODO: Check KNNMatcher vs. BFMatcher

		DMatch *bestMatch = nullptr;
		for (auto &match : matches)
		{
			if(bestMatch == nullptr)
				bestMatch = &match;
			else if(match.distance < bestMatch->distance)
				bestMatch = &match;
		}



		Point2f* mostLikelyFeaturePoint = &keypoints[bestMatch->queryIdx].pt;

		Mat result = currentFrame.clone();
		circle(result, *mostLikelyFeaturePoint, 3, Scalar(0, 0, 255), 6);


		vector<RotatedRect> contours = detectMovement(bufferFrame, currentFrame);
		for(const auto& contour : contours)
		{
			//rectangle(result, rect, Scalar(0, 255, 0, 2));
			Point2f rect_points[4]; contour.points(rect_points);
			for(int j = 0; j < 4; j++)
			{
				line(result, rect_points[j], rect_points[(j + 1) % 4], Scalar(0, 255, 0), 3, 8);
			}

			circle(result, contour.center, 3, Scalar(0, 255, 0), 5);
		}

		imshow("#result", result);

		//todo: A SHAPE IS RECOGNISED
		Shape shape(_sampleName);
		shape.SetCenter(*mostLikelyFeaturePoint);
		_receiver->ShapeDetected(*this, shape);
		

		//=========== Debug purposes
		//TODO: Label the movement blobs instead of just drawing circles
		//Mat result = current_frame.clone();
		//    for(auto &match : matches)
		//    {
		//        circle(result, keypoints[match.imgIdx].pt, 10, Scalar(0, 255, 0), 10);
		//    }

		//circle(result, keypoints[bestMatch->queryIdx].pt, 10, Scalar(0, 255, 0), 10);

		//imshow("Result " + _sampleName, result);

		_receiver->SignalEndFrame(*this);
	}
	else
	{
		// Sleep for 1 millisecond, since no new frame was pushed
		std::this_thread::sleep_for(std::chrono::milliseconds(1));
	}
}

void ObjectDetector::OnIncomingFrame(const Mat& frame) noexcept
{
	_bufferFrameMoveHelper.SetNewFrame(_currentFrameMoveHelper.GetCurrentFrameReference());
	_currentFrameMoveHelper.SetNewFrame(frame);
}