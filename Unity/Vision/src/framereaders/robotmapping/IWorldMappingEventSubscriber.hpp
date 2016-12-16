#pragma once

#include "ShapesTracker.hpp"

namespace robotmapping
{
	class IWorldMappingEventSubscriber
	{
		public:
			virtual ~IWorldMappingEventSubscriber() { };
			
			virtual void OnRecognise(const ShapesTracker& tracker, RecognisedShape& shape) noexcept = 0;
			virtual void OnLost(const ShapesTracker& tracker, RecognisedShape& shape) noexcept = 0;
			virtual void OnMove(const ShapesTracker& tracker, RecognisedShape& shape) noexcept = 0;
			virtual void OnVerticesChanged(const ShapesTracker& tracker, RecognisedShape& shape) noexcept = 0;
			
			virtual void SignalNewFrame(const ShapesTracker& tracker) noexcept = 0;
			virtual void SignalEndFrame(const ShapesTracker& tracker) noexcept = 0;
			virtual void ShapeDetected(const ShapesTracker& tracker, RecognisedShape& shape) noexcept = 0;
	}
}