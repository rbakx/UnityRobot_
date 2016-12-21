#pragma once

#include <vector>
#include <mutex>
#include <memory>

#include "VideoFeedFrameReceiver.hpp"

namespace frames
{
	/*
		VideoFeedFrameReceiverTargets is a segregation class for VideoFeedFrameReceiver.
		Instead of one receiver being able to receive a frame from a frame feeder, multiple targets can receive the frame.
		
		If any receive inherits from Runnable, Start and Stop will be called so their perspective processing can run along with receiving frames.
	*/
	class VideoFeedFrameReceiverTargets : public VideoFeedFrameReceiver
	{
		private:
			std::vector<std::shared_ptr<VideoFeedFrameReceiver>> _targets;
			std::mutex _lock;

			// Notifies all VideoFeedFrameReceiver in the _targets list of a new frame.
			void OnIncomingFrame(const cv::Mat& frame) noexcept;
		
		public:
			VideoFeedFrameReceiverTargets() noexcept;

			// Destructs the VideoFeedFrameReceiverTargets and calls the destructors of all VideoFeedFrameReceivers.
			// This is not the same as remove()
			~VideoFeedFrameReceiverTargets() noexcept;
			
			// Checks whether the VideoFeedFrameReceiver in question is already present in the _target list
			bool ContainsReceiver(const std::shared_ptr<VideoFeedFrameReceiver> target) const noexcept;
			
			/*
				Removes all shared points from _targets;
			*/
			void removeAll() noexcept;

			// Adds the VideoFeedFrameReceiver that is specified as an argument to the target list.
			// If the VideoFeedFrameReceiver is already present in the target list, this is reported and the
			// VideoFeedFrameReceiver is NOT added again.
			void add(std::shared_ptr<VideoFeedFrameReceiver> target) noexcept;

			// Adds the VideoFeedFrameReceivers that are specified in the vector to the target list.
			// If one of the VideoFeedFrameReceivers is already present in the target list, this is reported and the
			// VideoFeedFrameReceiver in question is NOT added again.
			void add(std::vector<std::shared_ptr<VideoFeedFrameReceiver>>& targets) noexcept;

			// Removes the VideoFeedFrameReceiver that is specified as an argument.
			// This receivers will STOP receiving new frames, but this function does NOT call the destructor of the target specified.
			// If any target in the list inherits Runnable, STOP is called after removal IF there are no other shared_pointers to the corresponding object.
			// When an non-existing target is passed as an argument, nothing will be removed and nothing is reported.
			void remove(const std::shared_ptr<VideoFeedFrameReceiver>& target) noexcept;

			// Removes the VideoFeedFrameReceivers that are specified in the vector from the target list
			// and therefore the receivers will STOP receiving new frames.
			// If target inherits Runnable, STOP is called after removal.
			// This function does NOT call the destructor of the target specified
			void remove(const std::vector<std::shared_ptr<VideoFeedFrameReceiver>>& targets) noexcept;

	};
}