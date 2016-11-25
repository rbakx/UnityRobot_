#include "VideoFeedFrameReceiverTargets.hpp"
#include <mutex>

using namespace std;
using namespace cv;
using namespace frames;

VideoFeedFrameReceiverTargets::VideoFeedFrameReceiverTargets() noexcept : _targets(vector<VideoFeedFrameReceiver*>())
{
	
}

size_t VideoFeedFrameReceiverTargets::getTargetIndexByPointer(VideoFeedFrameReceiver* target) const noexcept
{
	return (size_t)-1;
}

void VideoFeedFrameReceiverTargets::OnIncomingFrame(const Mat& frame) noexcept
{
	std::mutex lock;
	lock_guard<mutex> frame_guard(lock);
	for(auto videoFeedFrameReceiver : _targets)
	{
		videoFeedFrameReceiver->OnIncomingFrame(frame);
	}
}

void VideoFeedFrameReceiverTargets::add(VideoFeedFrameReceiver* target) noexcept
{
	if(target == nullptr)
		return;
	
	size_t index = getTargetIndexByPointer(target);
	
	if(index == ((size_t)-1))
	{
		_targets.push_back(target);
	}
}

void VideoFeedFrameReceiverTargets::remove(VideoFeedFrameReceiver* target) noexcept
{
	size_t index = getTargetIndexByPointer(target);
	
	if(index != ((size_t)-1))
	{
		_targets.erase(_targets.begin() + index);
	}
}