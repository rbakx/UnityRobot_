#ifndef VISION_ROBOT_DETECTION_H
#define VISION_ROBOT_DETECTION_H
#include <opencv2/opencv.hpp>
#include <opencv2/core/types.hpp>
#include "../Robot.h"

class RobotDetection
{
private:
    cv::Mat bufferFrame, currentFrame;
    cv::Ptr<cv::ORB> orb;
    cv::Ptr<cv::DescriptorMatcher> matcher;

    cv::Mat trainDescriptor;
    std::vector<cv::KeyPoint> trainKeypoints;
    cv::Mat trainSample;

    std::vector<Robot> robots;

    void processImage();
    void updateRobotPosition(int x, int y);

public:
    RobotDetection(const std::string fileName);
    void passNewFrame(const cv::Mat& frame);

    const std::vector<Robot>& getRobots() const;
};


#endif //VISION_ROBOT_DETECTION_H
