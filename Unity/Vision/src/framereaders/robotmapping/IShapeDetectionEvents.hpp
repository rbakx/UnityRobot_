#pragma once

#include "Shape.hpp"

namespace robotmapping
{
	class ShapeDetectorBase;
	
	class IShapeDetectionEvents
	{
		public:
		
			virtual ~IShapeDetectionEvents() {};
			
			virtual void SignalNewFrame(const ShapeDetectorBase& detector) noexcept = 0;
			virtual void SignalEndFrame(const ShapeDetectorBase& detector) noexcept = 0;
			
			virtual void ShapeDetected(const ShapeDetectorBase& detector, Shape& shape) noexcept = 0;
	};
}