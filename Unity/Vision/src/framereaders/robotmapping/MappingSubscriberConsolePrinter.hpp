#pragma once

#include "IWorldMappingEventSubscriber.hpp"

namespace robotmapping
{
	class MappingSubscriberConsolePrinter : public IWorldMappingEventSubscriber
	{
		public:
			void OnRecognise(const IShapeTrackers& tracker, Shape& shape) noexcept;
			void OnLost(const IShapeTrackers& tracker, Shape& shape) noexcept;
			void OnMove(const IShapeTrackers& tracker, Shape& shape) noexcept;
			void OnVerticesChanged(const IShapeTrackers& tracker, Shape& shape) noexcept;
			
			void SignalNewFrame(const IShapeTrackers& tracker) noexcept;
			void SignalEndFrame(const IShapeTrackers& tracker) noexcept;
			void ShapeDetected(const IShapeTrackers& tracker, Shape& shape) noexcept;
	};
}