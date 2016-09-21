#include "RobotDetection.h"

using namespace cv;
using namespace std;

void RobotDetection::updateRobotPosition(int x, int y)
{

}


RobotDetection::RobotDetection()
{

}

void RobotDetection::calibrate()
{

}

void RobotDetection::passNewFrame(const Mat& frame)
{
    bufferFrame = currentFrame.clone();
    currentFrame = frame.clone();

    if(bufferFrame.empty())
        return;


	Mat grayBuffer, grayCurrent;
	cvtColor(bufferFrame, grayBuffer, CV_BGR2GRAY);
	cvtColor(currentFrame, grayCurrent, CV_BGR2GRAY);

    Mat diffImage, thresholdImage;
    absdiff(grayBuffer, grayCurrent, diffImage);
	threshold(diffImage, thresholdImage, 10, 255, CV_THRESH_BINARY); //TODO: Make this adaptive? --> no.

	Mat filteredImage;
	erode(thresholdImage, filteredImage, Mat(), Point(-1, -1), 4);
	dilate(filteredImage, filteredImage, Mat(), Point(-1, -1), 25);

	imshow("Differences", thresholdImage);
	imshow("Filtered", filteredImage);

	Rect rect = boundingRect(filteredImage);

	Mat result = currentFrame.clone();
	rectangle(result, rect, Scalar(0, 255, 0), 3);
	imshow("Result", result);
}

const vector<Robot>& RobotDetection::getRobots() const
{
    return robots;
}
