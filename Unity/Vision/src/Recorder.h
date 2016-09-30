#ifndef ASSIGNMENT_RECORDER_H
#define ASSIGNMENT_RECORDER_H

#include <opencv2/opencv.hpp>
#include "libuvc/libuvc.h"

class Recorder {
private:
    const std::string DESTINATION = "result.avi";

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

public:
    Recorder();
    int run();

};

#endif //ASSIGNMENT_RECORDER_H
