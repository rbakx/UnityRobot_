#pragma once

#include "ShapeDetectorBase.hpp"
#include "../../Runnable.hpp"

#include "../FrameMoveThreadHelper.hpp"

#include <opencv2/opencv.hpp>
#include <opencv2/core/types.hpp>

namespace robotmapping
{
	class ObjectDetector : public ShapeDetectorBase, public Runnable
	{
		private:
			static const int NUMBER_OF_FEATURES = 200;
			static const int MINIMUM_BLOB_AREA = 40; //Area of the detected blobs in pixels

			const std::string _sampleName;

			FrameMoveThreadHelper _bufferFrameMoveHelper, _currentFrameMoveHelper;

			cv::Ptr<cv::ORB> _orb;
			cv::Ptr<cv::DescriptorMatcher> _matcher;

			cv::Mat _trainDescriptor;
			std::vector<cv::KeyPoint> _trainKeypoints;
			
			cv::Mat _trainSample;

			static std::vector<cv::RotatedRect> detectMovement(const cv::Mat& bufferFrame, const cv::Mat& currentFrame);
			void run();
		
		public:
		
			ObjectDetector(const ObjectDetector& copy) noexcept;
			ObjectDetector(IShapeDetectionEvents* receiver, const std::string& sampleName);
			~ObjectDetector();
		
			void OnIncomingFrame(const cv::Mat& frame) noexcept;
	};
}