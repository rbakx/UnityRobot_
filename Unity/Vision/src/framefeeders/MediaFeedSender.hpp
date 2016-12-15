#pragma once

#include <string>
#include "opencv2/opencv.hpp"
#include "../frames/VideoFeedFrameSender.hpp"

namespace framefeeders
{
    class MediaFeedSender : public frames::VideoFeedFrameSender
    {
        private:
			unsigned long _fps_capture_frame_delay_ns;
	        cv::VideoCapture _vc;

	        bool FeedReading() noexcept;

        public:
            MediaFeedSender(frames::VideoFeedFrameReceiver* target, const std::string& filePath);
			~MediaFeedSender();
    };
}