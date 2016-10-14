#include <stdexcept>

#include "VideoFeedFrameSender.hpp"

using namespace std;
using namespace cv;
using namespace frames;

VideoFeedFrameSender::VideoFeedFrameSender(VideoFeedFrameReceiver* target) noexcept : _target(target), _framesFeederThread(nullptr), _threadContinueRunning(true)
{
	if(_target == nullptr)
	{
		throw invalid_argument("[VideoFeedFrameSender] _target is nullptr");
	}

	_framesFeederThread = new thread([=]()
        {
            while(this->_threadContinueRunning)
            {
				this->FeedReading();
            }
        });
}

VideoFeedFrameSender::~VideoFeedFrameSender()
{
	if(_framesFeederThread->joinable())
	{
		_threadContinueRunning = false;
		_framesFeederThread->join();
	}

	delete _framesFeederThread;
}

void VideoFeedFrameSender::PushFrameToTarget(const Mat& frame) const noexcept
{
	_target->OnIncomingFrame(frame);
}