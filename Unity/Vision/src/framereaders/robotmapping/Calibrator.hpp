#pragma once

#include <string>
#include <vector>
#include "opencv2/features2d/features2d.hpp"
#include <opencv2/core/types.hpp>
#include "../../frames/VideoFeedFrameReceiver.hpp"

namespace robotmapping
{
	/*
		PRE: Calibrator uses a video feed to find features that may define robots.
		When calibration is complete, then the settings are stored on the disk.
	*/
	class Calibrator : public frames::VideoFeedFrameReceiver
	{
		private:
			cv::Mat _bufferFrame, _currentFrame;
			const int _NUMBER_OF_FEATURES = 20; //The maximum number of features to retain.
			cv::Ptr<cv::ORB> _orb;

			cv::Mat _ROImask;
			std::vector<cv::KeyPoint> _keypoints;
			cv::Mat _descriptors;

			bool _running;
			void updateROI();

		public:
			Calibrator();

			void Start();
			void Stop();

			void WriteToFile(const std::string &filePath);

			void OnIncomingFrame(const cv::Mat& frame) noexcept;
	};
}
