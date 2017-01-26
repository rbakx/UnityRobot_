//
// Created by rutger on 20-1-17.
//
#pragma once

#include "Shape.hpp"

namespace robotmapping
{
	class IShapeTracker
	{
		public:
			virtual void UpdateShape(const Shape& shape) noexcept = 0;
			virtual Shape::coordinate_type GetCenter() const noexcept = 0;
	};
}