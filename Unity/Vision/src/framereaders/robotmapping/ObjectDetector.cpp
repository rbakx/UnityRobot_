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

static void ObjectDetector::drawRotatedRect(cv::Mat& image, const cv::RotatedRect& rect)
{
	Point2f rect_points[4]; rect.points(rect_points);
	for(int j = 0; j < 4; j++)
	{
		line(image, rect_points[j], rect_points[(j + 1) % 4], Scalar(0, 255, 0), 3, 8);
	}
}

cv::Mat ObjectDetector::getROIFromRotRect(const cv::Mat& image, const cv::RotatedRect& rect)
{
	Mat M, rotated, cropped;

	// get angle and size from the bounding box
	float angle = rect.angle;

	Size rect_size = rect.size;
	// thanks to http://felix.abecassis.me/2011/10/opencv-rotation-deskewing/
	if (rect.angle < -45.) {
		angle += 90.0;
		swap(rect_size.width, rect_size.height);
	}


	M = getRotationMatrix2D(rect.center, angle, 1.0);

	// perform the affine transformation
	warpAffine(image, rotated, M, image.size(), INTER_CUBIC);

	// crop the resulting image
	getRectSubPix(rotated, rect_size, rect.center, cropped);

	return cropped;
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


		Mat result = currentFrame.clone();

		vector<RotatedRect> boundaries = detectMovement(bufferFrame, currentFrame);
		for(const auto& boundary : boundaries)
		{
			drawRotatedRect(result, boundary);

			circle(result, boundary.center, 3, Scalar(0, 255, 0), 5);
			

//			Mat ROI = getROIFromRotRect(currentFrame, boundary);
//
//			Mat queryDescriptor;
//			vector<KeyPoint> keypoints(20);
//
//			_orb->detect(ROI, keypoints);
//			_orb->compute(ROI, keypoints, queryDescriptor);
//
//			vector<DMatch> matches;
//			_matcher->match(queryDescriptor, _trainDescriptor, matches); //TODO: Check KNNMatcher vs. BFMatcher


			//todo: A SHAPE IS RECOGNISED
			Shape shape(_sampleName);
			shape.SetCenter(boundary.center);
			_receiver->ShapeDetected(*this, shape);
		}

		imshow("#result", result);

		

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