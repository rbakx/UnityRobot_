#include "VideoFrameDisplayer.hpp"

using namespace std;
using namespace cv;
using namespace framereaders;

VideoFrameDisplayer::VideoFrameDisplayer(const string& windowName,
										 const int windowWidth,
										 const int windowHeight)
		: _WINDOW_NAME(windowName),
		  _threadContinueRunning(true)
{
	namedWindow(_WINDOW_NAME, WINDOW_NORMAL);
	resizeWindow(_WINDOW_NAME, windowWidth, windowHeight);

	_displayThread = new thread([=]()
	 {
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
	{
		lock_guard<mutex> frame_guard(_lock);
		imshow(_WINDOW_NAME, _frame);
	}

	waitKey(1);
}