#include "MediaFeedSender.hpp"

#include <stdexcept>

using namespace std;
using namespace cv;
using namespace frames;
using namespace framefeeders;

MediaFeedSender::MediaFeedSender(VideoFeedFrameReceiver* target, const string& filePath)
		: VideoFeedFrameSender(target),
		  _fps_capture_frame_delay_ns(0),
		  _vc(VideoCapture(filePath))
{

	const double framerate = _vc.get(CV_CAP_PROP_FPS);
	_fps_capture_frame_delay_ns = 1000000000L / static_cast<unsigned long>(framerate);

	if(_fps_capture_frame_delay_ns <= 1000L || framerate <= 0.0F || framerate >= 1000.0F)
	{
		throw invalid_argument("[MediaFeedSender] No proper framerate (FPS) was found. Expected 1-999, got from inputted file : " +
			to_string(framerate));
	}

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
