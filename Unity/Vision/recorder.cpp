#include <stdio.h>
#include <opencv2/opencv.hpp>
#include "filters.h"

using namespace cv;
using namespace std;

/** Global variables */
static String imageFile = "./resources/edge_detection.jpg";

// const int WIDTH = 1920;
// const int HEIGHT = 1080;
// const double FPS = 24.0;

int main()
{
    VideoCapture cap(0); // open the external camera

    if(!cap.isOpened())  // check if we succeeded
        return -1;

    //Set video source to FullHD@30fps DOESNT WORK!
    // cap.set(CAP_PROP_FRAME_WIDTH, WIDTH);
    // cap.set(CAP_PROP_FRAME_HEIGHT, HEIGHT);
    // cap.set(CV_CAP_PROP_FPS, FPS);

    Size S = Size((int) cap.get(CV_CAP_PROP_FRAME_WIDTH),    // Acquire input size
    (int) cap.get(CV_CAP_PROP_FRAME_HEIGHT));

    int FPS = cap.get(CV_CAP_PROP_FPS);
    FPS = FPS < 1 ? 24 : FPS; //Set FPS to 24 if CV_CAP_PROP_FPS is not suppored

    VideoWriter writer("./resources/result.avi", CV_FOURCC('M', 'J', 'P', 'G'), FPS, S);

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
