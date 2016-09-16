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

    if(!bufferFrame.empty())
    {
        Mat diffImage;
        //diffImage = Mat::zeros(currentFrame.size(), currentFrame.type());
        absdiff(bufferFrame, currentFrame, diffImage);

        imshow("Differences", diffImage);
    }
}

const vector<Robot>& RobotDetection::getRobots() const
{
    return robots;
}
