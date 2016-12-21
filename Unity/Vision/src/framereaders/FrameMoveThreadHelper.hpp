#pragma once

#include <mutex>
#include <opencv2/core/mat.hpp>

namespace robotmapping
{
	class FrameMoveThreadHelper
	{
		private:
		
			std::mutex _lock;
			cv::Mat _frame;
			bool _ignore_new_till_accepted;
		
		public:
		
			FrameMoveThreadHelper() noexcept;
				
			~FrameMoveThreadHelper();
		
			bool HasFrame() noexcept;
			
			void SetNewFrame(const cv::Mat& new_frame);
			
			void IgnoreNewFramesIfPreviousNotAccepted(bool yes) noexcept;
			
			cv::Mat AcceptFrame() noexcept;
			
			/*
				Reset a frame, Warning, will NOT cause references to go out of scope, but will corrupt data being processed in multi-threaded applications.
			*/
			void Release() noexcept;
			
			/*
				Warning, might go out of scope when used together with SetNewFrame and/or AcceptFrame in multi-threaded applications.
			*/
			cv::Mat& GetCurrentFrameReference() noexcept;
	};
}