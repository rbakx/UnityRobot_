#pragma once

#include <opencv2/opencv.hpp>
#include <opencv2/core/types.hpp>
#include "../../Robot.h"
#include "../../frames/VideoFeedFrameReceiver.hpp"

namespace robotmapping
{
    class Detector : public frames::VideoFeedFrameReceiver
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

        public:
            Detector(const std::string fileName);

            const std::vector<Robot>& getRobots() const;

            void OnIncomingFrame(const cv::Mat& frame) noexcept;
    };
}