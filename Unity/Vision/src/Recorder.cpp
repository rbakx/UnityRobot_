#include "Recorder.h"

#ifdef __linux__

#include <sys/stat.h>
#include "libusb-1.0/libusb.h"
#endif


using namespace cv;
using namespace std;

Recorder::Recorder(Settings &settings)
    : deviceNumber(settings.getDeviceProperties().number),
      vid(settings.getDeviceProperties().vid),
      pid(settings.getDeviceProperties().pid),
      width(settings.getRecordingProperties().width),
      height(settings.getRecordingProperties().height),
      fps(settings.getRecordingProperties().fps)
{
    setAutoFocus();
}

int Recorder::run()
{
    VideoCapture cap(deviceNumber); // open the external camera

    if(!cap.isOpened())  // check if we succeeded
    {
        cout << "Camera failed to open!" << endl;
        return -1;
    }

    //Set video source to FullHD@24fps
    //cap.set(CV_CAP_PROP_FOURCC, CODEC);
    cap.set(CV_CAP_PROP_FRAME_WIDTH, width);
    cap.set(CV_CAP_PROP_FRAME_HEIGHT, height);
    cap.set(CV_CAP_PROP_FPS, fps); //Warning: This doesn't work for at least the Logitech C930e
    //cap.set(CV_CAP_PROP_AUTOFOCUS, 0); //Warning: This doesn't work for at least the Logitech C930e

    VideoWriter writer(DESTINATION, CODEC, fps, Size(width, height));

    if (!writer.isOpened())
    {
        cout << "Could not open the output video for write" << endl;
        return -1;
    }

    Mat frame;

    while(true)
    {
        cap >> frame;

        writer.write(frame);

        imshow("Original", frame); //show original

        if (waitKey(fps) == 27) //Display images in 30fps and when ASCII key 27 (ESC) is pressed, quit application
            break;
    }

    cout << "Wrote recording to " << DESTINATION << endl;
    return 0;
}


void Recorder::setAutoFocus()
{
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
            vid, pid, NULL); /* filter devices: vendor_id, product_id, "serial_num" */

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

/*void Recorder::setWritePermission()
{
    libusb_context *context;
    libusb_device_handle *dev_handle;
    libusb_device *dev;

    int result = libusb_init(&context);

    if(result != 0) {
        cout << "Unable to initialize libusb" << endl;
        return;
    }


    dev_handle = libusb_open_device_with_vid_pid(context, VID, PID);

    if(dev_handle == NULL) {
        cout << "Unable to get a device handle for device " << hex << VID << ":" << hex << PID << endl;
        return;
    }

    dev = libusb_get_device(dev_handle);

    int busNumber = libusb_get_bus_number(dev);
    int portNumber = libusb_get_port_number(dev);

    //Done getting the bus and port number, so we can close the handles
    libusb_close(dev_handle);

    ostringstream filePathStream;
    filePathStream << "/dev/bus/usb/" << busNumber << "/" << portNumber;

    int chmod_status = chmod(filePathStream.str().c_str(), S_IWOTH);
    if(chmod_status < 0) {
        cout << "Granting writing permissions failed." << endl;
        return;
    }
}*/