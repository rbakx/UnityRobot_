//
// Created by rutger on 19-1-17.
//

#include "KalmanShapeTracker.hpp"

using namespace robotmapping;

KalmanShapeTracker::KalmanShapeTracker()
{
	_kalmanFilter.init(_NUMBER_OF_STATES, _NUMBER_OF_MEASURMENTS, _NUMBER_OF_INPUTS, _TYPE);

	cv::setIdentity(_kalmanFilter.measurementMatrix);							// because we don't know, send help

	cv::setIdentity(_kalmanFilter.processNoiseCov, cv::Scalar::all(1e-5));		// set process noise
	cv::setIdentity(_kalmanFilter.measurementNoiseCov, cv::Scalar::all(1e-4));	// set measurement noise
	cv::setIdentity(_kalmanFilter.errorCovPost, cv::Scalar::all(1));			// error covariance
//
//	float transitionMatrixData[16] = {
//			1,	0,	1,	0,
//			0,	1,	0,	1,
//			0,	0,	1,	0,
//			0,	0,	0,	1
//	};
//
//	_kalmanFilter.transitionMatrix = cv::Mat(4, 4, _TYPE, transitionMatrixData);

	_lastMeasurement = cv::Mat(2, 1, _TYPE, 0.0f); //Declared here to work around constant new memory allocation
}

KalmanShapeTracker::KalmanShapeTracker(const KalmanShapeTracker& other)
{
	_kalmanFilter = other._kalmanFilter;
	other._lastMeasurement.copyTo(_lastMeasurement);
	other._estimated.copyTo(_estimated);
}


//TODO: Return status bool
void KalmanShapeTracker::UpdateShape(const Shape& shape) noexcept
{
	_lastMeasurement.at<float>(0) = shape.Center().x;
	_lastMeasurement.at<float>(1) = shape.Center().y;

	/*
	 * TODO:	Currently, we only use Kalman filters to track the center of the Shape object.
	 *			We want to track the rotation of Shapes as well, which can be achieved by adding
	 *			the contours of the Shape to the lastMeasurement matrix.
	 */

	_kalmanFilter.predict();
	_estimated = _kalmanFilter.correct(_lastMeasurement);
}

Shape::coordinate_type KalmanShapeTracker::GetCenter() const noexcept
{
	return cv::Point2f(
			_estimated.at<float>(0),
			_estimated.at<float>(1)
	);
}