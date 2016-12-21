#pragma once

#include "ShapeDetectorBase.hpp"
#include "IShapeDetectionEvents.hpp"

namespace robotmapping
{
	class ShapesTracker : public IShapeDetectionEvents
	{
		private:
			
			void SignalNewFrame(const ShapeDetectorBase& detector) noexcept;
			void SignalEndFrame(const ShapeDetectorBase& detector) noexcept;
			
			void ShapeDetected(const ShapeDetectorBase& detector, Shape& shape) noexcept;
		
		public:
		
			ShapesTracker();
			virtual ~ShapesTracker();
	};
}