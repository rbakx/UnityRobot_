#include "MappingSubscriberConsolePrinter.hpp"

#include <iostream>

using namespace robotmapping;

void MappingSubscriberConsolePrinter::OnRecognise(const IShapeTrackers& tracker, Shape& shape) noexcept
{
	std::cout << "[ShapesTracker] NEW MATCH: (" << shape.GetTrackerId() << ") " << shape.ToString() << std::endl;
}

void MappingSubscriberConsolePrinter::OnLost(const IShapeTrackers& tracker, Shape& shape) noexcept
{
	std::cout << "[ShapesTracker] I LOST IT, BRO! (" << shape.GetTrackerId() << ") " << shape.ToString() << std::endl;
}

void MappingSubscriberConsolePrinter::OnMove(const IShapeTrackers& tracker, Shape& shape) noexcept
{
	std::cout << "[ShapesTracker] POSITION UPDATE: (" << shape.GetTrackerId() << ") " << shape.ToString() << std::endl;
}

void MappingSubscriberConsolePrinter::OnVerticesChanged(const IShapeTrackers& tracker, Shape& shape) noexcept
{
	
}

void MappingSubscriberConsolePrinter::SignalNewFrame(const IShapeTrackers& tracker) noexcept
{
	
}

void MappingSubscriberConsolePrinter::SignalEndFrame(const IShapeTrackers& tracker) noexcept
{
	
}

void MappingSubscriberConsolePrinter::ShapeDetected(const IShapeTrackers& tracker, Shape& shape) noexcept
{
	
}