#include "VideoFrameDisplayer.hpp"

using namespace std;
using namespace cv;
using namespace framereaders;

void VideoFrameDisplayer::run()
{
	if(_hasNewFrame)
	{
		lock_guard<mutex> frame_guard(_lock);
		_hasNewFrame = false;

		if(_frame.empty())
			return;

		imshow(_WINDOW_NAME, _frame);

		waitKey(1);
	}
}

VideoFrameDisplayer::VideoFrameDisplayer(const string& windowName,
										 int windowWidth,
										 int windowHeight)
		: _WINDOW_NAME(windowName),
		  _WINDOW_WIDTH(windowWidth),
		  _WINDOW_HEIGHT(windowHeight)
{
	namedWindow(_WINDOW_NAME, WINDOW_NORMAL);
	resizeWindow(_WINDOW_NAME, _WINDOW_WIDTH, _WINDOW_HEIGHT);
}

VideoFrameDisplayer::~VideoFrameDisplayer()
{
	destroyWindow(_WINDOW_NAME);
	Stop();
}

void VideoFrameDisplayer::OnIncomingFrame(const Mat& frame) noexcept
{
	lock_guard<mutex> frame_guard(_lock);
	_frame = frame.clone();
	_hasNewFrame = true;
}