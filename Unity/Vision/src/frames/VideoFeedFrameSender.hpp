#pragma once

#include <thread>
#include "VideoFeedFrameReceiver.hpp"

namespace frames
{
	class VideoFeedFrameSender
	{
		private:
		
			VideoFeedFrameReceiver* _target;
			std::thread* _framesFeederThread;
			bool _threadContinueRunning;
			
			virtual bool FeedReading() noexcept = 0;
			/*
				POST: FeedReading is a method called by the constructor of the VideoFeedFrameSender.
				It is started in a separate thread which manages its own lifetime.
				
				RETURNS bool : If an unrecoverable error occured, bool should return false.
				
				NOTE: Initialisation and de-initalisation should be managed by the derived class!
			*/

			void stopFeederReaderThread() noexcept;
			
		protected:
		
			void PushFrameToTarget(const cv::Mat& frame) const noexcept;
		    void signalObjectsSetup() noexcept;
			void signalObjectsAboutToDestructed() noexcept;
		
		public:
		
			VideoFeedFrameSender(VideoFeedFrameReceiver* target) noexcept;
			~VideoFeedFrameSender();
	};
}