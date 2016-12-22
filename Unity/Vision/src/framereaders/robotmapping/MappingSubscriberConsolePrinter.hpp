#pragma once

#include "IWorldMappingEventSubscriber.hpp"

namespace robotmapping
{
	class MappingSubscriberConsolePrinter : public IWorldMappingEventSubscriber
	{
		public:
		
			void OnRecognise(const ShapesTracker& tracker, Shape& shape) noexcept;
			void OnLost(const ShapesTracker& tracker, Shape& shape) noexcept;
			void OnMove(const ShapesTracker& tracker, Shape& shape) noexcept;
			void OnVerticesChanged(const ShapesTracker& tracker, Shape& shape) noexcept;
			
			void SignalNewFrame(const ShapesTracker& tracker) noexcept;
			void SignalEndFrame(const ShapesTracker& tracker) noexcept;
			void ShapeDetected(const ShapesTracker& tracker, Shape& shape) noexcept;
	};
}