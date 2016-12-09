#include "VideoFeedFrameReceiverTargets.hpp"
#include <mutex>

using namespace std;
using namespace cv;
using namespace frames;

VideoFeedFrameReceiverTargets::VideoFeedFrameReceiverTargets() noexcept : _targets(vector<VideoFeedFrameReceiver*>())
{}

bool VideoFeedFrameReceiverTargets::isReceiverPresent(const VideoFeedFrameReceiver * const target) const noexcept
{
	return std::find(_targets.begin(), _targets.end(), target) != _targets.end();
}

void VideoFeedFrameReceiverTargets::OnIncomingFrame(const Mat& frame) noexcept
{
	lock_guard<mutex> frame_guard(_lock);
	for(VideoFeedFrameReceiver * const videoFeedFrameReceiver : _targets)
	{
		videoFeedFrameReceiver->OnIncomingFrame(frame);
	}
}

void VideoFeedFrameReceiverTargets::add(VideoFeedFrameReceiver * const target) noexcept
{
	if(isReceiverPresent(target))
	{
		cout << "[VideoFeedFrameReceiverTargets] Tried to add already present receiver." << endl;
		return;
	}

	lock_guard<mutex> frame_guard(_lock);
	_targets.push_back(target);
}

void VideoFeedFrameReceiverTargets::add(const vector<VideoFeedFrameReceiver*>& targets) noexcept
{
	for(auto target : targets)
	{
		add(target);
	}
}

void VideoFeedFrameReceiverTargets::remove(const VideoFeedFrameReceiver * const target)
{
	lock_guard<mutex> frame_guard(_lock);
	_targets.erase(std::remove(_targets.begin(), _targets.end(), target), _targets.end());
}

void VideoFeedFrameReceiverTargets::remove(const vector<VideoFeedFrameReceiver*>& targets) noexcept
{
	for(auto target : targets)
	{
		remove(target);
	}
}