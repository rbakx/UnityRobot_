#pragma once

#include <opencv2/opencv.hpp>

namespace frames
{
	/*
		Interface class for receiving frames from frame feeders.
	*/
	class VideoFeedFrameReceiver
	{
		public:
		
			virtual ~VideoFeedFrameReceiver() {};
			
			/*
				Note: OnIncomingFrame is called with the frame in question as a reference.
				When the method returns, consider 'frame' to be out of scope.
				Do not copy the reference locally for later use; Instead, make a copy.
				For moving a frame from OnIncomingFrame to own thread, please have a look at the classes
				Runnable and FrameMoveThreadHelper.
			*/
			virtual void OnIncomingFrame(const cv::Mat& frame) noexcept = 0;
	};
}