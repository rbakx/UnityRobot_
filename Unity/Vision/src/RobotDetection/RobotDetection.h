#ifndef VISION_ROBOT_DETECTION_H
#define VISION_ROBOT_DETECTION_H
#include <opencv2/opencv.hpp>
#include "../Robot.h"

class RobotDetection
{
private:
    cv::Mat bufferFrame, currentFrame;
    std::vector<Robot> robots;

    void updateRobotPosition(int x, int y);

public:
    RobotDetection();
    void calibrate();
    void passNewFrame(const cv::Mat& frame);

    const std::vector<Robot>& getRobots() const;
};


#endif //VISION_ROBOT_DETECTION_H
