#include "MediaFeedSender.hpp"

#include <stdexcept>
#include <iostream>

using namespace std;
using namespace cv;
using namespace frames;
using namespace framefeeders;

MediaFeedSender::MediaFeedSender(VideoFeedFrameReceiver* target, const string& filePath)
		: VideoFeedFrameSender(target),
		  _fps_capture_frame_delay_ns(0L),
		  _vc(VideoCapture(filePath))
{

	const double framerate = _vc.get(CV_CAP_PROP_FPS);
	_fps_capture_frame_delay_ns = 1000000000L / static_cast<unsigned long>(framerate);

	if(_fps_capture_frame_delay_ns <= 100000L || framerate < 1.0F || framerate > 9999.0F)
	{
		throw invalid_argument("[MediaFeedSender] No proper framerate (FPS) was found. Expected 1-9999, got from inputted file : " +
			to_string(framerate));
	}
	
	/*
		Count off one millisecond for waitKey command in base
	*/
	if(_fps_capture_frame_delay_ns < 1000000L)
	{ _fps_capture_frame_delay_ns = 0; } //value of 0 will stop thread from sleeping at all, but this must happen due to frequency.
	else
	{ _fps_capture_frame_delay_ns -= 1000000L; }

	signalObjectsSetup();
}

MediaFeedSender::~MediaFeedSender()
{
	signalObjectsAboutToDestructed();
}

bool MediaFeedSender::FeedReading() noexcept
{
	Mat frame;
	_vc >> frame;

	if(frame.empty())
	{
		return false;
	}

	PushFrameToTarget(frame);
	this_thread::sleep_for(chrono::nanoseconds(_fps_capture_frame_delay_ns));

	return true;
}
