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
	
	// Call waitKey once, so all configuration and window initialising can happen
	waitKey(1);
	
	_framesFeederThread = new thread([=]()
	{
		while (this->_threadContinueRunning)
		{
			this->FeedReading();
			/*
				Manditory waitKey so any receiving frames can also execute openCV functions
			*/
			waitKey(1);
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