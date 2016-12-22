#pragma once

#include "ShapesTracker.hpp"
#include "Shape.hpp"

namespace robotmapping
{
	class IWorldMappingEventSubscriber
	{
		public:
			virtual ~IWorldMappingEventSubscriber() { };
			
			virtual void OnRecognise(const ShapesTracker& tracker, Shape& shape) noexcept = 0;
			virtual void OnLost(const ShapesTracker& tracker, Shape& shape) noexcept = 0;
			virtual void OnMove(const ShapesTracker& tracker, Shape& shape) noexcept = 0;
			virtual void OnVerticesChanged(const ShapesTracker& tracker, Shape& shape) noexcept = 0;
			
			virtual void SignalNewFrame(const ShapesTracker& tracker) noexcept = 0;
			virtual void SignalEndFrame(const ShapesTracker& tracker) noexcept = 0;
			virtual void ShapeDetected(const ShapesTracker& tracker, Shape& shape) noexcept = 0;
	};
}