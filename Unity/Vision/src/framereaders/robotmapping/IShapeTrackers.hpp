//
// Created by rutger on 20-1-17.
//
#pragma once

#include "IShapeDetectionEvents.hpp"

namespace robotmapping
{
	class IWorldMappingEventSubscriber;

	class IShapeTrackers : public IShapeDetectionEvents
	{
	protected:
		IWorldMappingEventSubscriber* _subscriber;
	private:

		virtual void SignalNewFrame(const ShapeDetectorBase& detector) noexcept = 0;
		virtual void SignalEndFrame(const ShapeDetectorBase& detector) noexcept = 0;
		virtual void ShapeDetected(const ShapeDetectorBase& detector, Shape& shape) noexcept = 0;

	public:
		IShapeTrackers(IWorldMappingEventSubscriber* subscriber) : _subscriber(subscriber)
		{
			if(_subscriber == nullptr)
			{
				throw std::invalid_argument("[ShapeTrackers] subscriber is null");
			}
		};

		virtual ~IShapeTrackers() {};
	};
}