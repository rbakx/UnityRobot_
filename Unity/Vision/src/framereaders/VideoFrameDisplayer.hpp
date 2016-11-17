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
			cv::Mat _frame;
			std::mutex _lock;
			bool newFrame;

			std::thread* _displayThread;
			bool _threadContinueRunning;

			void threadDisplayMethod() noexcept;

		public:
			VideoFrameDisplayer(const std::string& windowName = "Display",
								const int windowWidth = 640,
								const int windowHeight = 350);

			virtual ~VideoFrameDisplayer();

			void OnIncomingFrame(const cv::Mat &frame) noexcept;
	};
}