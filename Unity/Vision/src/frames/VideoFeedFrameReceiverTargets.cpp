#include "VideoFeedFrameReceiverTargets.hpp"

#include "../Runnable.hpp"

using namespace std;
using namespace cv;
using namespace frames;

VideoFeedFrameReceiverTargets::VideoFeedFrameReceiverTargets() noexcept
		: _targets(vector<shared_ptr<VideoFeedFrameReceiver>>())
{}

VideoFeedFrameReceiverTargets::~VideoFeedFrameReceiverTargets() noexcept
{ removeAll(); }

void VideoFeedFrameReceiverTargets::removeAll() noexcept
{
	for(auto target : _targets)
	{
		/* Stop any class that inherits from Runnable from running, may not be the case at all.
		   This is here for convenience sake */
		   
		if(target.use_count() == 1)
		{
			Runnable* run = dynamic_cast<Runnable*>(target.get());
			if(run != nullptr) { run->Stop(); }
		}
	}
	
	_targets.clear();
}

bool VideoFeedFrameReceiverTargets::ContainsReceiver(const shared_ptr<VideoFeedFrameReceiver> target) const noexcept
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
	if(ContainsReceiver(target))
	{
		cout << "[VideoFeedFrameReceiverTargets] Tried to add already present receiver." << endl;
		return;
	}
	
	/* Start any class that inherits from Runnable, doesn't have to the case at all.
	   This is here for convenience sake */
	   
	Runnable* run = dynamic_cast<Runnable*>(target.get());
	if(run != nullptr) { run->Start(); }

	
	lock_guard<mutex> frame_guard(_lock);
	_targets.push_back(target);
}

void VideoFeedFrameReceiverTargets::add(vector<shared_ptr<VideoFeedFrameReceiver>>& targets) noexcept
{
	for(auto target : targets)
	{ this->add(target); }
}

void VideoFeedFrameReceiverTargets::remove(const shared_ptr<VideoFeedFrameReceiver>& target) noexcept
{
	auto it = find_if(_targets.begin(), _targets.end(), [&target](const shared_ptr<VideoFeedFrameReceiver>& obj) {return obj == target;});

	if (it != _targets.end())
	{	
		_targets.erase(it);
		
		/* Stop any class that inherits from Runnable from running, may not be the case at all.
		   This is here for convenience sake */
		   
		if(target.use_count() == 1)
		{
			Runnable* run = dynamic_cast<Runnable*>(target.get());
			if(run != nullptr) { run->Stop(); }
		}
	}
}

void VideoFeedFrameReceiverTargets::remove(const vector<shared_ptr<VideoFeedFrameReceiver>>& targets) noexcept
{
	for(auto target : targets)
	{
		this->remove(target);
	}
}