#include "MappingSubscriberConsolePrinter.hpp"

#include <iostream>

using namespace robotmapping;

void MappingSubscriberConsolePrinter::OnRecognise(const ShapesTracker& tracker, Shape& shape) noexcept
{
	std::cout << "[ShapesTracker] NEW MATCH: (" << shape.GetTrackingId() << ") " << shape.ToString() << std::endl;
}

void MappingSubscriberConsolePrinter::OnLost(const ShapesTracker& tracker, Shape& shape) noexcept
{
	std::cout << "[ShapesTracker] I LOST IT, BRO! (" << shape.GetTrackingId() << ") " << shape.ToString() << std::endl;
}

void MappingSubscriberConsolePrinter::OnMove(const ShapesTracker& tracker, Shape& shape) noexcept
{
	std::cout << "[ShapesTracker] POSITION UPDATE: (" << shape.GetTrackingId() << ") " << shape.ToString() << std::endl;
}

void MappingSubscriberConsolePrinter::OnVerticesChanged(const ShapesTracker& tracker, Shape& shape) noexcept
{
	
}

void MappingSubscriberConsolePrinter::SignalNewFrame(const ShapesTracker& tracker) noexcept
{
	
}

void MappingSubscriberConsolePrinter::SignalEndFrame(const ShapesTracker& tracker) noexcept
{
	
}

void MappingSubscriberConsolePrinter::ShapeDetected(const ShapesTracker& tracker, Shape& shape) noexcept
{
	
}