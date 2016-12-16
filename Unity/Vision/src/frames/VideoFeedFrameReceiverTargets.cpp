#include "VideoFeedFrameReceiverTargets.hpp"

using namespace std;
using namespace cv;
using namespace frames;

VideoFeedFrameReceiverTargets::VideoFeedFrameReceiverTargets() noexcept
		: _targets(vector<shared_ptr<VideoFeedFrameReceiver>>())
{}

VideoFeedFrameReceiverTargets::~VideoFeedFrameReceiverTargets() noexcept
{}

bool VideoFeedFrameReceiverTargets::isReceiverPresent(const shared_ptr<VideoFeedFrameReceiver> target) const noexcept
{
	return std::find(_targets.begin(), _targets.end(), target) != _targets.end();
}

void VideoFeedFrameReceiverTargets::OnIncomingFrame(const Mat& frame) noexcept
{
	lock_guard<mutex> frame_guard(_lock);
	for(auto videoFeedFrameReceiver : _targets)
	{
		videoFeedFrameReceiver->OnIncomingFrame(frame);
	}
}


void VideoFeedFrameReceiverTargets::add(shared_ptr<VideoFeedFrameReceiver> target) noexcept
{
	if(isReceiverPresent(target))
	{
		cout << "[VideoFeedFrameReceiverTargets] Tried to add already present receiver." << endl;
		return;
	}

	lock_guard<mutex> frame_guard(_lock);
	_targets.push_back(target);
}

void VideoFeedFrameReceiverTargets::add(vector<shared_ptr<VideoFeedFrameReceiver>>& targets) noexcept
{
	for(auto target : targets)
	{
		this->add(target);
	}
}

void VideoFeedFrameReceiverTargets::remove(const shared_ptr<VideoFeedFrameReceiver> target) noexcept
{
	_targets.erase(std::remove(_targets.begin(), _targets.end(), target), _targets.end());
}

void VideoFeedFrameReceiverTargets::remove(const vector<shared_ptr<VideoFeedFrameReceiver>>& targets) noexcept
{
	for(auto target : targets)
	{
		this->remove(target);
	}
}