#include "ShapesTracker.hpp"

using namespace robotmapping;

ShapesTracker::ShapesTracker()
{
	
}

ShapesTracker::~ShapesTracker()
{
	
}

void ShapesTracker::SignalNewFrame(const ShapeDetectorBase& detector) noexcept
{
	std::cout << "[ShapesTracker] FRAME NEW" << std::endl;
}

void ShapesTracker::SignalEndFrame(const ShapeDetectorBase& detector) noexcept
{
	std::cout << "[ShapesTracker] FRAME END" << std::endl;
}

void ShapesTracker::ShapeDetected(const ShapeDetectorBase& detector, Shape& shape) noexcept
{
	std::cout << "[ShapesTracker] NEW SHAPE: " << shape.ToString() << std::endl;
}