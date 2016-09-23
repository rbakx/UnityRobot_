#include <stdio.h>
#include <opencv2/opencv.hpp>
#include "libuvc/libuvc.h"

using namespace cv;
using namespace std;

const string DESTINATION = "result.avi";

//Opens the Logitech C930e which is assumed to be the second connected camera
//(as most laptops have a webcam as first camera)
const int DEVICE_NR = 1;
const int VID = 0x046d;
const int PID = 0x0843;

const int WIDTH = 1920;
const int HEIGHT = 1080;
const double FPS = 24;
const int CODEC = CV_FOURCC('M', 'J', 'P', 'G');

void setAutoFocus();

int main()
{
    setAutoFocus();

    VideoCapture cap(DEVICE_NR); // open the external camera

    if(!cap.isOpened())  // check if we succeeded
    {
        cout << "Camera failed to open!" << endl;
        return -1;
    }

    //Set video source to FullHD@24fps
    cap.set(CV_CAP_PROP_FOURCC, CODEC);
    cap.set(CV_CAP_PROP_FRAME_WIDTH, WIDTH);
    cap.set(CV_CAP_PROP_FRAME_HEIGHT, HEIGHT);
    //cap.set(CV_CAP_PROP_FPS, FPS); //Warning: This doesn't work for at least the Logitech C930e
    //cap.set(CV_CAP_PROP_AUTOFOCUS, 0); //Warning: This doesn't work for at least the Logitech C930e


    VideoWriter writer(DESTINATION, CODEC, FPS, Size(WIDTH, HEIGHT));

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

        if (waitKey(FPS) == 27) //Display images in 30fps and when ASCII key 27 (ESC) is pressed, quit application
            break;
    }

    return 0;
}


void setAutoFocus()
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
            VID, PID, NULL); /* filter devices: vendor_id, product_id, "serial_num" */

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