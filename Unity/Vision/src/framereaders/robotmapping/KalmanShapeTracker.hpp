//
// Created by rutger on 19-1-17.
//
#pragma once
#include <opencv2/core/types.hpp>
#include <opencv2/video/tracking.hpp>
#include "Shape.hpp"
#include "IShapeTracker.hpp"

namespace robotmapping
{
	class KalmanShapeTracker : public IShapeTracker
	{
		private:
			/*
			 * Kalman Filter specific declarations below, based on http://docs.opencv.org/3.1.0/dc/d2c/tutorial_real_time_pose.html
			 */
			const unsigned int _TYPE = CV_32F;		//We have to use one fixed matrix type throughout our calculations
			const int _NUMBER_OF_STATES = 3;		//We have positional and rotational data (x,y,yaw)
			const int _NUMBER_OF_MEASURMENTS = 2;	//We will only be able to measure (x,y) data
			const int _NUMBER_OF_INPUTS = 0;		//The number of action control (?)

			cv::KalmanFilter _kalmanFilter;
			cv::Mat _lastMeasurement;
			cv::Mat _estimated;

		public:
			KalmanShapeTracker();
			KalmanShapeTracker(const KalmanShapeTracker& other);

			void UpdateShape(const Shape& shape) noexcept;
			Shape::coordinate_type GetCenter() const noexcept;
	};
}