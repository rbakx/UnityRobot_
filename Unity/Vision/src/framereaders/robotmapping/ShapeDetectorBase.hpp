#pragma once

#include "IShapeDetectionEvents.hpp"
#include "../../VideoFeedFrameReceiver.hpp"

namespace robotmapping
{	
	class ShapeDetectorBase : public frames::VideoFeedFrameReceiver
	{
		protected:
		
			IShapeDetectionEvents* _receiver;
		
		public:
		
		ShapeDetectorBase(IShapeDetectionEvents* receiver) noexcept;
		
		virtual ~ShapeDetectorBase();
		
		
		virtual void OnIncomingFrame(const cv::Mat& frame) noexcept = 0;
	}
}