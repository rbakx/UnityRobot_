#include "VideoFrameDisplayer.hpp"

using namespace std;
using namespace cv;
using namespace framereaders;

VideoFrameDisplayer::VideoFrameDisplayer(const string& windowName,
										 int windowWidth,
										 int windowHeight)
		: Runnable(),
		  _WINDOW_NAME(windowName),
		  _WINDOW_WIDTH(windowWidth),
		  _WINDOW_HEIGHT(windowHeight)
{
	
}

VideoFrameDisplayer::~VideoFrameDisplayer()
{
	/*
		First stop the 'run' thread, then destroy the window.
		the run method in this class spawns the named window!
	*/
	Stop();
	
	destroyWindow(_WINDOW_NAME);
}

void VideoFrameDisplayer::run()
{	
	if(_frame_move_helper.HasFrame())
	{
		namedWindow(_WINDOW_NAME, WINDOW_NORMAL);
		resizeWindow(_WINDOW_NAME, _WINDOW_WIDTH, _WINDOW_HEIGHT);
		
		Mat frame = _frame_move_helper.AcceptFrame();

		imshow(_WINDOW_NAME, frame);

		/*
			waitKey(1) - no need for a waitKey here, as the supplier of the frames already has a waiting mechanism;
			Without multi-threading support enabled, this call was mandatory
		*/
	}
	else
	{
		// Sleep for 1 millisecond, since no new frame was pushed
		std::this_thread::sleep_for(std::chrono::milliseconds(1));
	}
}

void VideoFrameDisplayer::OnIncomingFrame(const Mat& frame) noexcept
{
	_frame_move_helper.SetNewFrame(frame);
}