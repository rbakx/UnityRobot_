#ifdef __linux__
    #include <sys/stat.h>
    #include "libusb-1.0/libusb.h"
#endif

#include <stdexcept>

#include "libuvc/libuvc.h"

#include "CameraFeedSender.hpp"

using namespace cv;
using namespace std;

using namespace frames;
using namespace framefeeders;

CameraFeedSender::CameraFeedSender(VideoFeedFrameReceiver* target) : VideoFeedFrameSender(target),
	  _cap(VideoCapture(settings->getDeviceProperties().number)),
      _vid(settings->getDeviceProperties().vid),
      _pid(settings->getDeviceProperties().pid),
      _width(settings->getRecordingProperties().width),
      _height(settings->getRecordingProperties().height),
      _fps(settings->getRecordingProperties().fps),
	  _fps_capture_frame_delay_ns(1000000000L / _fps)

{

	if(_fps_capture_frame_delay_ns <= 1000L || _fps <= 0.0F || _fps >= 1000.0F)
	{
		throw invalid_argument("[CameraFeedSender] No proper framerate (FPS) was found. Expected 1-999, got from settings : " + to_string(_fps));
	}

	if(_width <= 0 || _height <= 0)
	{
		throw invalid_argument("[CameraFeedSender] No proper recording dimensions (width, height) were found.\n"
									   "Expected bigger than 0, got from settings: " + to_string(_width) + ", " + to_string(_height));
	}

	if(_vid == -1 || _pid == -1)
	{
		throw invalid_argument("[CameraFeedSender] No valid VID and/or PID were entered.\n"
									   "Expected bigger than 0, got from settings: " + to_string(_vid) + ", " + to_string(_pid));
	}

	//TODO: Make a bool in config.yml to enable/disable autofocus as it might be not supported for the webcam
	if(!settings->getRecordingProperties().autofocus)
    	disableAutoFocus();
	
	if(!_cap.isOpened())  // check if we succeeded
    {
        throw runtime_error("[CameraFeedSender] Camera at index " + to_string(settings->getDeviceProperties().number) + " could not be opened!");
    }
		
    //Set video source to FullHD@24fps(
    //cap.set(CV_CAP_PROP_FOURCC, CODEC);
    _cap.set(CV_CAP_PROP_FRAME_WIDTH, _width);
    _cap.set(CV_CAP_PROP_FRAME_HEIGHT, _height);
    _cap.set(CV_CAP_PROP_FPS, _fps); //Warning: This doesn't work for at least the Logitech C930e
    //cap.set(CV_CAP_PROP_AUTOFOCUS, 0); //Warning: This doesn't work for at least the Logitech C930e

    signalObjectsSetup();
}

CameraFeedSender::~CameraFeedSender()
{
	signalObjectsAboutToDestructed();
}

void CameraFeedSender::disableAutoFocus()
{
	/*
		PRE:
		UVC is a library used to find a usb video device, to disable change autofocus.
	*/
    uvc_context_t *ctx;
    uvc_device_handle_t *devh;
    uvc_device_t *dev;
    uvc_error_t res;

    res = uvc_init(&ctx, NULL);

    if (res < 0) {
        uvc_perror(res, "uvc_init");
        return;
    }

    /* Locates the attached UVC device, stores in dev */
    res = uvc_find_device(
            ctx, &dev,
            _vid, _pid, NULL); /* filter devices: vendor_id, product_id, "serial_num" */

    if (res < 0) {
        uvc_perror(res, "uvc_find_device"); /* no devices found */
        return;
    }


        /* Try to open the device: requires exclusive access */
    res = uvc_open(dev, &devh);

    if (res < 0) {
        uvc_perror(res, "uvc_open"); /* unable to open device */
        return;
    }


    res = uvc_set_focus_auto(devh, 0);

    uvc_close(devh);

    /* Release the device descriptor */
    uvc_unref_device(dev);
    /* Close the UVC context. This closes and cleans up any existing device handles,
    * and it closes the libusb context if one was not provided. */
    uvc_exit(ctx);
}

bool CameraFeedSender::FeedReading() noexcept
{
    Mat frame;
    _cap >> frame;

    if(!frame.empty())
    {
        PushFrameToTarget(frame);
    }

    this_thread::sleep_for(chrono::nanoseconds(_fps_capture_frame_delay_ns));
	
	return true;
}
