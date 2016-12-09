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
		  ShouldStop(false)
{
	namedWindow(_WINDOW_NAME, WINDOW_NORMAL);
	resizeWindow(_WINDOW_NAME, _WINDOW_WIDTH, _WINDOW_HEIGHT);
}

VideoFrameDisplayer::VideoFrameDisplayer(const VideoFrameDisplayer& copy)
	: _WINDOW_NAME(copy._WINDOW_NAME),
	  _WINDOW_WIDTH(copy._WINDOW_WIDTH),
	  _WINDOW_HEIGHT(copy._WINDOW_HEIGHT),
	  _frame(copy._frame),
	  _newFrame(copy._newFrame),
	  ShouldStop(copy.ShouldStop)
{}

VideoFrameDisplayer::~VideoFrameDisplayer()
{
	destroyWindow(_WINDOW_NAME);
	ShouldStop = true;
}

void VideoFrameDisplayer::OnIncomingFrame(const Mat& frame) noexcept
{
	lock_guard<mutex> frame_guard(_lock);
	_frame = frame.clone();
	_newFrame = true;
}

void VideoFrameDisplayer::operator()()
{
	while(!ShouldStop)
	{
		if(_newFrame)
		{
			lock_guard<mutex> frame_guard(_lock);
			_newFrame = false;

			if(_frame.empty())
				continue;

			imshow(_WINDOW_NAME, _frame);

			waitKey(1);
		}
	}
}