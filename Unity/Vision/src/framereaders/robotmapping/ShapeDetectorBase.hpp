#pragma once

#include <memory>

#include "IShapeDetectionEvents.hpp"
#include "../../frames/VideoFeedFrameReceiver.hpp"

namespace robotmapping
{	
	class ShapeDetectorBase : public frames::VideoFeedFrameReceiver
	{
		protected:
			IShapeDetectionEvents* _receiver;
		
		public:
			ShapeDetectorBase(const ShapeDetectorBase& base) noexcept;
			ShapeDetectorBase(IShapeDetectionEvents* receiver) noexcept;

			virtual ~ShapeDetectorBase();

			virtual void OnIncomingFrame(const cv::Mat& frame) noexcept = 0;

			static std::vector<std::shared_ptr<frames::VideoFeedFrameReceiver>> createAndStartDetectorsFromSettings(IShapeDetectionEvents& receiver) noexcept;
	};
}