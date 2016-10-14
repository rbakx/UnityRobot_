#pragma once

#include <vector>

#include "VideoFeedFrameReceiver.hpp"

namespace frames
{
	class VideoFeedFrameReceiverTargets : public VideoFeedFrameReceiver
	{
		private:
			std::vector<VideoFeedFrameReceiver*> _targets;
			
			size_t getTargetIndexByPointer(VideoFeedFrameReceiver* target) const noexcept;
			void OnIncomingFrame(const cv::Mat& frame) noexcept;
		
		public:
			VideoFeedFrameReceiverTargets() noexcept;

			void add(VideoFeedFrameReceiver* target) noexcept;
			void remove(VideoFeedFrameReceiver* target) noexcept;
	};
}