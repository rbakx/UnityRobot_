#pragma once

#include <string>
#include <mutex>
#include <thread>
#include <condition_variable>
#include <opencv2/core/mat.hpp>
#include "../frames/VideoFeedFrameReceiver.hpp"

namespace framereaders
{
	class VideoFrameDisplayer : public frames::VideoFeedFrameReceiver
	{
		private:
			const std::string _WINDOW_NAME;
			const int _WINDOW_WIDTH, _WINDOW_HEIGHT;
			cv::Mat _frame;
			std::mutex _lock;
			bool _newFrame;

		public:
			VideoFrameDisplayer(const std::string& windowName  = "Display",
								int windowWidth = 640,
								int windowHeight = 350);

			VideoFrameDisplayer(const VideoFrameDisplayer&);
			virtual ~VideoFrameDisplayer();

			void OnIncomingFrame(const cv::Mat &frame) noexcept;
			void operator()();

			bool ShouldStop;
	};
}