#pragma once

#include <vector>
#include <mutex>
#include "VideoFeedFrameReceiver.hpp"

namespace frames
{
	class VideoFeedFrameReceiverTargets : public VideoFeedFrameReceiver
	{
		private:
			std::vector<VideoFeedFrameReceiver*> _targets;
			std::mutex _lock;


			// Checks whether the VideoFeedFrameReceiver in question is already present in the _target list
			bool isReceiverPresent(const VideoFeedFrameReceiver * const target) const noexcept;

			// Notifies all VideoFeedFrameReceiver in the _targets list of a new frame.
			void OnIncomingFrame(const cv::Mat& frame) noexcept;
		
		public:
			VideoFeedFrameReceiverTargets() noexcept;

			// Destructs the VideoFeedFrameReceiverTargets and calls the destructors of all VideoFeedFrameReceivers.
			// This is not the same as remove()
			~VideoFeedFrameReceiverTargets() noexcept;

			// Adds the VideoFeedFrameReceiver that is specified as an argument to the target list.
			// If the VideoFeedFrameReceiver is already present in the target list, this is reported and the
			// VideoFeedFrameReceiver is NOT added again.
			void add(VideoFeedFrameReceiver * const target) noexcept;

			// Adds the VideoFeedFrameReceivers that are specified in the vector to the target list.
			// If one of the VideoFeedFrameReceivers is already present in the target list, this is reported and the
			// VideoFeedFrameReceiver in question is NOT added again.
			void add(const std::vector<VideoFeedFrameReceiver*>& targets) noexcept;

			// Removes the VideoFeedFrameReceiver that is specified as an argument.
			// This receivers will STOP receiving new frames, but this function does NOT call the destructor of the target specified
			// When an non-existing target is passed as an argument, nothing will be removed and nothing is reported.
			void remove(const VideoFeedFrameReceiver * const target) noexcept;

			// Removes the VideoFeedFrameReceivers that are specified in the vector from the target list
			// and therefore the receivers will STOP receiving new frames.
			// This function does NOT call the destructor of the target specified
			void remove(const std::vector<VideoFeedFrameReceiver*>& targets) noexcept;
	};
}