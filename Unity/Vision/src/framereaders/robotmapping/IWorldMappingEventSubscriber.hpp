#pragma once

#include "IShapeTrackers.hpp"
#include "Shape.hpp"

namespace robotmapping
{
	class IWorldMappingEventSubscriber
	{
		public:
			virtual ~IWorldMappingEventSubscriber() { };

			virtual void OnRecognise(const IShapeTrackers& tracker, Shape& shape) noexcept = 0;
			virtual void OnLost(const IShapeTrackers& tracker, Shape& shape) noexcept = 0;
			virtual void OnMove(const IShapeTrackers& tracker, Shape& shape) noexcept = 0;
			virtual void OnVerticesChanged(const IShapeTrackers& tracker, Shape& shape) noexcept = 0;
			
			virtual void SignalNewFrame(const IShapeTrackers& tracker) noexcept = 0;
			virtual void SignalEndFrame(const IShapeTrackers& tracker) noexcept = 0;
			virtual void ShapeDetected(const IShapeTrackers& tracker, Shape& shape) noexcept = 0;
	};
}