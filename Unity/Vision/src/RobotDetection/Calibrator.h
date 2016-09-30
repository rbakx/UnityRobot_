#ifndef ASSIGNMENT_CALIBRATOR_H
#define ASSIGNMENT_CALIBRATOR_H

#include <string>
#include <vector>
#include "../Recorder.h"
#include "opencv2/features2d/features2d.hpp"
#include <opencv2/core/types.hpp>

class Calibrator {
private:
    std::string inputFilePath;
    Recorder *recorder;

    cv::Mat bufferFrame, currentFrame;
    int numberOfFeatures = 20; //The maximum number of features to retain.
    cv::Ptr<cv::ORB> orb;

    std::vector<cv::Mat> ROIs;
    std::vector<std::vector<cv::KeyPoint>> sampleKeypoints;
    std::vector<cv::Mat> descriptors;


    void passNewFrame(const cv::Mat &img);
    void findROIs();


public:
    Calibrator(const std::string inputFilePath);
    Calibrator(const Recorder recorder);
    void writeToFile(const std::string filePath) const;
};


#endif //ASSIGNMENT_CALIBRATOR_H
