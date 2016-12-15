#pragma once

#include <opencv2/opencv.hpp>
#include <mutex>

namespace frames
{
	class VideoFeedFrameReceiver
	{
		protected:
			std::mutex _lock;
			bool _hasNewFrame;

		public:
			VideoFeedFrameReceiver() : _hasNewFrame(false) {};
			virtual ~VideoFeedFrameReceiver() {};
			virtual void OnIncomingFrame(const cv::Mat& frame) noexcept = 0;
	};
}