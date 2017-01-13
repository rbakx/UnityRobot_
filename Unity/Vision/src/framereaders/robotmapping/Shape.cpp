#include "Shape.hpp"

#include <sstream>

using namespace std;
using namespace cv;
using namespace robotmapping;

const std::string Shape::UNKNOWN_PROPERTY = "[UNDEF]";

Shape::Shape(const Shape& copy) noexcept :
	_main_property(copy._main_property),
	_tracking_id(copy._tracking_id),
	_euler_angles(copy._euler_angles),
	_center(copy._center),
	_shape_data(copy._shape_data)
{
	
}

Shape::Shape(std::string main_property, unsigned long tracking_id) noexcept :
	_main_property(move(main_property)),
	_tracking_id(tracking_id)
{
	
}

void Shape::SetAngles(Shape::rotation_angles_type euler_angles) noexcept
{
	_euler_angles = std::move(euler_angles);
}

Shape::coordinate_type Shape::Center() const noexcept
{
	return _center;
}

void Shape::SetCenter(coordinate_type center_point) noexcept
{
	_center = std::move(center_point);
}

void Shape::SetShapeData(std::vector<Shape::coordinate_type> vertices_data)
{
	_shape_data = std::move(vertices_data);
}

std::vector<Shape::coordinate_type> Shape::GetShapeData() noexcept
{
	return _shape_data;
}

const std::vector<Shape::coordinate_type>& Shape::GetShapeDataReference()
{
	return _shape_data;
}

unsigned long Shape::GetTrackerId() const noexcept
{
	return _tracking_id;
}

void Shape::SetTrackerId(unsigned long trackerId) noexcept
{
	_tracking_id = trackerId;
}

string Shape::ToString() noexcept
{
	stringstream s;
	
	s << "center: [" << _center.x << ", " << _center.y << "]";
	
	return s.str();  
}

bool robotmapping::operator==(const Shape& s, const unsigned long tracker_id)
{
	return s.GetTrackerId() == tracker_id;
}