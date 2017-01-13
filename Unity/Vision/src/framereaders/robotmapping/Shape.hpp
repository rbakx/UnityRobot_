#pragma once

#include <string>
#include <vector>
#include <opencv2/opencv.hpp>

namespace robotmapping
{
	class Shape
	{
		public:
		
			typedef cv::Point2f coordinate_type;
			typedef cv::Point3f rotation_angles_type;
			static const std::string UNKNOWN_PROPERTY;
		
		private:
			
			std::string _main_property;	
			unsigned long _tracking_id;
		
			rotation_angles_type _euler_angles;
			coordinate_type _center;
			
			std::vector<coordinate_type> _shape_data;
		
		public:
		
			Shape(const Shape& copy) noexcept;
			Shape(std::string main_property = UNKNOWN_PROPERTY, unsigned long tracking_id = 0) noexcept;
			
			coordinate_type Center() const noexcept;
			
			void SetAngles(rotation_angles_type euler_angles) noexcept;
			
			void SetCenter(coordinate_type center_point) noexcept;
			
			/*
				Override the current shape data of this shape.
				Note: Be sure to order all points as if drawing one straight line instead of chaotic!
				I.E. Convex hull points.
			*/
			void SetShapeData(std::vector<coordinate_type> vertices_data);
			
			/*
				Return a copy of the current shape data.
			*/
			std::vector<coordinate_type> GetShapeData() noexcept;
			
			/*
				Returns a reference instead of a copy of the current shape data.
				A reference could go out of scope when Shape does, but is much more efficient for calculations.
			*/
			const std::vector<coordinate_type>& GetShapeDataReference();
			
			std::string ToString() noexcept;
			
			unsigned long GetTrackerId() const noexcept;
			void SetTrackerId(unsigned long new_id) noexcept;
	};
	
	bool operator==(const Shape& s, const unsigned long tracker_id);
}