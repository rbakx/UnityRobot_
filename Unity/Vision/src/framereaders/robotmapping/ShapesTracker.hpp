#pragma once

#include <vector>
#include <atomic>

#include "ShapeDetectorBase.hpp"
#include "IShapeDetectionEvents.hpp"

namespace robotmapping
{
	class IWorldMappingEventSubscriber;
	
	class ShapesTracker : public IShapeDetectionEvents
	{
		private:
			
			void SignalNewFrame(const ShapeDetectorBase& detector) noexcept;
			void SignalEndFrame(const ShapeDetectorBase& detector) noexcept;
			
			void ShapeDetected(const ShapeDetectorBase& detector, Shape& shape) noexcept;

			std::vector<Shape> _tracked_shapes;
			std::vector<Shape> _new_frame_shapes;
			
			IWorldMappingEventSubscriber* _subscriber;
			
			std::atomic<unsigned long> _tracker_id_top;
		
		public:
		
			ShapesTracker(IWorldMappingEventSubscriber* subscriber);
			virtual ~ShapesTracker();
	};
}