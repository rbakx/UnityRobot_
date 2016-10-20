#include "Detector.hpp"
#include "../../Settings.h"

using namespace cv;
using namespace std;
using namespace robotmapping;

void Detector::processImage()
{
    Mat queryDescriptor;

    vector<KeyPoint> keypoints;
    _orb->detect(_currentFrame, keypoints);
	_orb->compute(_currentFrame, keypoints, queryDescriptor);

    vector<DMatch> matches;
    _matcher->match(queryDescriptor, _trainDescriptor, matches);

    DMatch *bestMatch = nullptr;
    for(auto &match : matches)
    {
        if(bestMatch == nullptr)
            bestMatch = &match;
        else if(match.distance < bestMatch->distance)
            bestMatch = &match;
    }

    //TODO: Label the movement blobs instead of just drawing circles
    Mat result = _currentFrame.clone();
//    for(auto &match : matches)
//    {
//        circle(result, keypoints[match.imgIdx].pt, 10, Scalar(0, 255, 0), 10);
//    }

    circle(result, keypoints[bestMatch->queryIdx].pt, 10, Scalar(0, 255, 0), 10);

	imshow("Result", result);
}

Detector::Detector()
{
    _orb = ORB::create();

	string fileName = settings->getGeneralProperties().sampleName;

	if(strcmp(fileName.c_str(), "") == 0)
		throw invalid_argument("[Detector] sampleName in config.yml is invalid.");

    FileStorage fs2(fileName, FileStorage::READ);

	if(!fs2.isOpened())
		throw invalid_argument("[Detector] sample provided in config.yml is inaccessible.");

    FileNode descriptorNode = fs2["Descriptors"];
    FileNode keypointsNode = fs2["Keypoints"];
    FileNode sampleNode = fs2["Sample"];

    if(descriptorNode.isNone() || keypointsNode.isNone() || sampleNode.isNone()) //We couldn't find the descriptors in the file
        throw runtime_error("Sample is not complete!");

    read(descriptorNode, _trainDescriptor);
    read(keypointsNode, _trainKeypoints);
    read(sampleNode, _trainSample);

    fs2.release();

    cout << "Read sample from file" << endl;

    _matcher = new cv::BFMatcher(cv::NORM_HAMMING, true);
}

const vector<Robot>&Detector::getRobots() const
{
	/*
	 * TODO: This doesn't work yet, as we are currently only labeling blobs.
	 * 		 We need to implement this next
	 */
    return _robots;
}

void Detector::OnIncomingFrame(const Mat& frame) noexcept
{
    _bufferFrame = _currentFrame.clone();
    _currentFrame = frame.clone();

    if(_bufferFrame.empty())
        return;

    processImage();
}