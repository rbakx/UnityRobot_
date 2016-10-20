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
            cv::Mat _bufferFrame, _currentFrame;
            cv::Ptr<cv::ORB> _orb;
            cv::Ptr<cv::DescriptorMatcher> _matcher;

            cv::Mat _trainDescriptor;
            std::vector<cv::KeyPoint> _trainKeypoints;
            cv::Mat _trainSample;

            std::vector<Robot> _robots;

            void processImage();

        public:
            Detector();

            const std::vector<Robot>& getRobots() const;

            void OnIncomingFrame(const cv::Mat& frame) noexcept;
    };
}