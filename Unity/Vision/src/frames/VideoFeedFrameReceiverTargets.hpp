#pragma once

#include <vector>
#include "VideoFeedFrameReceiver.hpp"

namespace frames
{
	class VideoFeedFrameReceiverTargets : public VideoFeedFrameReceiver
	{
		private:
			std::vector<VideoFeedFrameReceiver*> _targets;

			bool isReceiverPresent(const VideoFeedFrameReceiver * const target) const noexcept;
			void OnIncomingFrame(const cv::Mat& frame) noexcept;
		
		public:
			VideoFeedFrameReceiverTargets() noexcept;

			void add(VideoFeedFrameReceiver * const target) noexcept;
			void add(const std::vector<VideoFeedFrameReceiver*>& targets) noexcept;
			void remove(const VideoFeedFrameReceiver * const target);
			void remove(const std::vector<VideoFeedFrameReceiver*>& targets) noexcept;
	};
}