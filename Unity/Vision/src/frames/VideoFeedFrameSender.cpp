#include <stdexcept>

#include "VideoFeedFrameSender.hpp"

using namespace std;
using namespace cv;
using namespace frames;

VideoFeedFrameSender::VideoFeedFrameSender(VideoFeedFrameReceiver* target) : _target(target), _framesFeederThread(nullptr), _threadContinueRunning(false)
{
	if(_target == nullptr)
	{
		throw invalid_argument("[VideoFeedFrameSender] _target is nullptr");
	}
}

void VideoFeedFrameSender::signalObjectsSetup() noexcept
{
	stopFeederReaderThread();

	_threadContinueRunning = true;
	_framesFeederThread = new thread([=]() {
		while (this->_threadContinueRunning)
		{
			this->FeedReading();
		}
	});
}

void VideoFeedFrameSender::signalObjectsAboutToDestructed() noexcept
{
	stopFeederReaderThread();
}

void VideoFeedFrameSender::stopFeederReaderThread() noexcept
{
	if(_framesFeederThread != nullptr && _framesFeederThread->joinable())
	{
		_threadContinueRunning = false;
		_framesFeederThread->join();
	}

	delete _framesFeederThread;

	_framesFeederThread = nullptr;
}

VideoFeedFrameSender::~VideoFeedFrameSender()
{
	stopFeederReaderThread();
}

void VideoFeedFrameSender::PushFrameToTarget(const Mat& frame) const noexcept
{
	_target->OnIncomingFrame(frame);
}