#pragma once

#include <vector>
#include <atomic>

#include "ShapeDetectorBase.hpp"
#include "IShapeDetectionEvents.hpp"
#include "IShapeTracker.hpp"
#include "IShapeTrackers.hpp"

#include "IWorldMappingEventSubscriber.hpp"

namespace robotmapping
{
	template <class T>
	class ShapeTrackers : public IShapeTrackers
	{
		private:
			std::vector<T> _shapeTrackers;

			/*
			 * Specific to finding which shape should be fed to which tracking filter
			 */
			const float _TOLERANCE = 65.0;
			std::vector<Shape> _new_frame_shapes;

			static float getDistance(const cv::Point2f& p1, const cv::Point2f& p2) noexcept
			{
				return sqrt( pow(p2.x - p1.x, 2) + pow(p2.y - p1.y, 2) );
			};

			T* findMostLikelyTracker(const Shape& shape) noexcept
			{
				float smallestDistance = std::numeric_limits<float>::max();
				T* mostLikelyShapeTracker_ptr = nullptr;

				for(auto& tracker : _shapeTrackers)
				{
					const float distance = getDistance(shape.Center(), tracker.GetCenter());

					if(distance < smallestDistance)
					{
						mostLikelyShapeTracker_ptr = &tracker;
						smallestDistance = distance;
					}
				}

				if(smallestDistance < _TOLERANCE)
					return mostLikelyShapeTracker_ptr;
				else
					return nullptr;
			};

			virtual void SignalNewFrame(const ShapeDetectorBase& detector) noexcept
			{
				_subscriber->SignalNewFrame(*this);
			};

			virtual void SignalEndFrame(const ShapeDetectorBase& detector) noexcept
			{
				for(auto shape : _new_frame_shapes)
				{
					T* shapeTracker_ptr = findMostLikelyTracker(shape);

					if(shapeTracker_ptr == nullptr)
					{
						T st;
						st.UpdateShape(shape);

						_shapeTrackers.push_back(st);

						_subscriber->OnRecognise(*this, shape);
					}
					else
					{
						shapeTracker_ptr->UpdateShape(shape);

					}
				}

				_new_frame_shapes.clear();

				_subscriber->SignalEndFrame(*this);
			};
			
			virtual void ShapeDetected(const ShapeDetectorBase& detector, Shape& shape) noexcept
			{
				_new_frame_shapes.push_back(shape);
				_subscriber->ShapeDetected(*this, shape);
			};

	public:
			ShapeTrackers(IWorldMappingEventSubscriber* subscriber) : IShapeTrackers(subscriber)
			{ };
			virtual ~ShapeTrackers() {};
	};
}