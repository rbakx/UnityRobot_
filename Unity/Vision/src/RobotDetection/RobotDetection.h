#ifndef VISION_ROBOT_DETECTION_H
#define VISION_ROBOT_DETECTION_H
#include <opencv2/opencv.hpp>
#include <opencv2/core/types.hpp>
#include "../Robot.h"

class RobotDetection
{
private:
    cv::Mat bufferFrame, currentFrame;
    //cv::Ptr<cv::ORB> orb;
    cv::Ptr<cv::DescriptorMatcher> matcher;
    std::vector<std::vector<cv::KeyPoint>> sampleKeypoints;
    std::vector<cv::Mat> descriptors;

    std::vector<Robot> robots;

    void processImage();
    void updateRobotPosition(int x, int y);

public:
    RobotDetection();
    void passNewFrame(const cv::Mat& frame);

    const std::vector<Robot>& getRobots() const;
};


#endif //VISION_ROBOT_DETECTION_H
