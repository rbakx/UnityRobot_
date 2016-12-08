#include "Calibrator.hpp"
#include "../../Settings.h"

using namespace std;
using namespace cv;
using namespace robotmapping;

Calibrator::Calibrator()
	: _running(false)
{
    _orb = ORB::create(_NUMBER_OF_FEATURES);
}

void Calibrator::updateROI()
{
	if(_bufferFrame.empty())
		return;

	Mat grayBuffer, grayCurrent, filteredImage;

	cvtColor(_bufferFrame, grayBuffer, CV_BGR2GRAY);
	cvtColor(_currentFrame, grayCurrent, CV_BGR2GRAY);

	absdiff(grayBuffer, grayCurrent, filteredImage);
	threshold(filteredImage, filteredImage, 10, 255, CV_THRESH_BINARY); //TODO: Make this adaptive? --> no. Document dis

	medianBlur(filteredImage, filteredImage, 5);

	if(countNonZero(filteredImage) < 1) //Check if there is something moving, otherwise boundingRect will throw an exception
		return;

	if(_ROImask.rows <= 0)
		_ROImask = Mat::zeros(_currentFrame.size(), CV_8U);

	bitwise_or(_ROImask, filteredImage, _ROImask); //Merge current mask with overall ROImask
}

void Calibrator::Start()
{
	_running = true;
}

void Calibrator::Stop()
{
	_running = false;
}

void Calibrator::WriteToFile(const string& fileName)
{
	_orb->detect(_currentFrame, _keypoints, _ROImask);
	_orb->compute(_currentFrame, _keypoints, _descriptors);

	string fileLocation = settings->getFilePath() + "samples/" + fileName;
    FileStorage fs(fileLocation, FileStorage::WRITE);

    write(fs, "Descriptors", _descriptors);
    write(fs, "Keypoints", _keypoints);
    write(fs, "Sample", Mat(_currentFrame.clone(), boundingRect(_ROImask)));

    fs.release();
}

void Calibrator::OnIncomingFrame(const cv::Mat& frame) noexcept
{
	_bufferFrame = _currentFrame.clone();
	_currentFrame = frame.clone();

	if(_running)
		updateROI();
}