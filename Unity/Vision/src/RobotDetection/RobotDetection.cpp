#include "RobotDetection.h"

using namespace cv;
using namespace std;

void RobotDetection::processImage()
{
	Mat grayBuffer, grayCurrent;
	cvtColor(bufferFrame, grayBuffer, CV_BGR2GRAY);
	cvtColor(currentFrame, grayCurrent, CV_BGR2GRAY);

	Mat diffImage, thresholdImage;
	absdiff(grayBuffer, grayCurrent, diffImage);
	threshold(diffImage, thresholdImage, 10, 255, CV_THRESH_BINARY); //TODO: Make this adaptive? --> no.

	Mat filteredImage;
	medianBlur(thresholdImage, filteredImage, 9);

	Mat structuredElement = getStructuringElement(CV_SHAPE_ELLIPSE, Size(3, 3));
	dilate(filteredImage, filteredImage, structuredElement, Point(-1, -1), 25);

	imshow("Differences", thresholdImage);
	imshow("Filtered", filteredImage);



	if(countNonZero(filteredImage) < 1) //Check if there is something moving, otherwise boundingRect will throw an exception
		return;

	Rect rect = boundingRect(filteredImage);

	Mat result = currentFrame.clone();
	rectangle(result, rect, Scalar(0, 255, 0), 3);


	if(rect.width >= rect.height)
		circle(result, Point(rect.x + rect.height / 2, rect.y + rect.height / 2), 10, Scalar(0, 0, 255), 20);
	else
		circle(result, Point(rect.x + rect.width / 2, rect.y + rect.width / 2), 10, Scalar(0, 0, 255), 20);

	imshow("Result", result);
}

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

	processImage();
}

const vector<Robot>& RobotDetection::getRobots() const
{
    return robots;
}
