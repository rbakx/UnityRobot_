#pragma once

#include <vector>
#include <atomic>

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

			std::vector<Shape> _tracked_shapes;
			std::vector<Shape> _new_frame_shapes;
			
			std::atomic<unsigned long> tracker_id_top;
		
		public:
		
			ShapesTracker();
			virtual ~ShapesTracker();
	};
}