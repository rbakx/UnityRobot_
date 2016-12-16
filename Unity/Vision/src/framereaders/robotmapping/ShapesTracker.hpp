#pragma once

#include "ShapeDetectorBase.hpp"
#include "IShapeDetectionEvents.hpp"

namespace robotmapping
{
	class ShapesTracker
	{
		virtual ~ShapesTracker();
		
		void Add(ShapeDetectorBase* detector) noexcept;
		void Remove(ShapeDetectorBase* detector) noexcept;
			
		void SignalNewFrame(const ShapeDetectorBase& detector) noexcept;
		void SignalEndFrame(const ShapeDetectorBase& detector) noexcept;
		
		void ShapeDetected(const ShapeDetectorBase& detector, RecognisedShape& shape) noexcept;
	}
}