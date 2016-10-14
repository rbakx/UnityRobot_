#pragma once

#include <string>
#include <opencv2/opencv.hpp>

#include "../frames/VideoFeedFrameReceiver.hpp"

namespace framereaders
{
	class VideoFrameSaver : public frames::VideoFeedFrameReceiver
	{
	private:
		cv::VideoWriter _writer;

		int width;
		int height;
		double fps;

		const int CODEC = CV_FOURCC('M', 'J', 'P', 'G');

	public:
		VideoFrameSaver();

		~VideoFrameSaver();

		void StartSaving(const std::string &filename) noexcept;

		void StopSaving() noexcept;

		void OnIncomingFrame(const cv::Mat &frame) noexcept;
	};
}