#pragma once

#include <opencv2/opencv.hpp>

#include "../Settings.h"
#include "../frames/VideoFeedFrameSender.hpp"

namespace framefeeders
{
	class CameraFeedSender : public frames::VideoFeedFrameSender
	{
	private:
			//Opens the Logitech C930e which is assumed to be the second connected camera
			//(as most laptops have a webcam as first camera)
			int _vid;
			int _pid;

			int _width;
			int _height;
			double _fps;
			const unsigned long _fps_capture_frame_delay_ns;
			
			cv::VideoCapture _cap; // open the external camera

			void disableAutoFocus();
		
			bool FeedReading() noexcept;		
		
		public:
		
			CameraFeedSender(frames::VideoFeedFrameReceiver* target);
		  	~CameraFeedSender();
	};
}