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
		
			FrameMoveThreadHelper _frame_move_helper;
		
			const std::string _sampleName;

			cv::Mat _bufferFrame, _currentFrame;
			cv::Ptr<cv::ORB> _orb;
			cv::Ptr<cv::DescriptorMatcher> _matcher;

			cv::Mat _trainDescriptor;
			std::vector<cv::KeyPoint> _trainKeypoints;
			
			cv::Mat _trainSample;
		
			void run();
		
		public:
		
			ObjectDetector(const ObjectDetector& copy) noexcept;
			ObjectDetector(IShapeDetectionEvents* receiver, const std::string& sampleName);
			~ObjectDetector();
		
			void OnIncomingFrame(const cv::Mat& frame) noexcept;
	};
}