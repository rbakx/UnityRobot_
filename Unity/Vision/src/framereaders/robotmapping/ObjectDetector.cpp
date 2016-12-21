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

void ObjectDetector::run()
{	
	if(_frame_move_helper.HasFrame())
	{
		Mat current_frame = _frame_move_helper.AcceptFrame();
		
		_receiver->SignalNewFrame(*this);

		Mat queryDescriptor;

		vector<KeyPoint> keypoints(200);
		
		/*
		    ORB is a very intensive process!
			Can take about 10% on a i7-6700HQ
		*/
		_orb->detect(current_frame, keypoints);
		_orb->compute(current_frame, keypoints, queryDescriptor);

		vector<DMatch> matches;
		_matcher->match(queryDescriptor, _trainDescriptor, matches);

		DMatch *bestMatch = nullptr;
		for (auto &match : matches)
		{
			if(bestMatch == nullptr)
				bestMatch = &match;
			else if(match.distance < bestMatch->distance)
				bestMatch = &match;
		}
		
		//todo: A SHAPE IS RECOGNISED
		Shape shape(_sampleName);
		shape.SetCenter(keypoints[bestMatch->queryIdx].pt);
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
		
		
		/*
			waitKey(1) - no need for a waitKey here, as the supplier of the frames already has a waiting mechanism;
			Without multi-threading support enabled, this call was mandatory
		*/
		
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
	_frame_move_helper.SetNewFrame(frame);
}