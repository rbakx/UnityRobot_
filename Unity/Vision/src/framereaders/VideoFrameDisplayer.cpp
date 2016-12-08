#include "VideoFrameDisplayer.hpp"

using namespace std;
using namespace cv;
using namespace framereaders;

VideoFrameDisplayer::VideoFrameDisplayer(const string& windowName,
										 int windowWidth,
										 int windowHeight)
		: _WINDOW_NAME(windowName),
		  _WINDOW_WIDTH(windowWidth),
		  _WINDOW_HEIGHT(windowHeight),
		  _threadContinueRunning(true)
{
	_displayThread = new thread([=]()
	{
		namedWindow(_WINDOW_NAME, WINDOW_NORMAL);
		resizeWindow(_WINDOW_NAME, _WINDOW_WIDTH, _WINDOW_HEIGHT);
		while(this->_threadContinueRunning)
		{
			if(newFrame)
			{
				newFrame = false;
				this->threadDisplayMethod();
			}
		}
	});
}

VideoFrameDisplayer::~VideoFrameDisplayer()
{
	if(_displayThread->joinable())
	{
		_threadContinueRunning = false;
		_displayThread->join();
		destroyWindow(_WINDOW_NAME);
	}

	delete _displayThread;
}

void VideoFrameDisplayer::OnIncomingFrame(const Mat& frame) noexcept
{
	lock_guard<mutex> frame_guard(_lock);
	_frame = frame.clone();
	newFrame = true;
}

void VideoFrameDisplayer::threadDisplayMethod() noexcept
{
	if (_frame.empty())
		return;

	{
		lock_guard<mutex> frame_guard(_lock);
		imshow(_WINDOW_NAME, _frame);
	}

	waitKey(1);
}