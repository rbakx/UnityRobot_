#pragma once

#include <opencv2/opencv.hpp>

namespace frames
{
	class VideoFeedFrameReceiver
	{
		public:
			virtual ~VideoFeedFrameReceiver() {};
			virtual void OnIncomingFrame(const cv::Mat& frame) noexcept = 0;
	};
}