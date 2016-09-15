#include <stdio.h>
#include <opencv2/opencv.hpp>

using namespace cv;
using namespace std;

const string DESTINATION = "result.avi";

const int WIDTH = 1920;
const int HEIGHT = 1080;
const double FPS = 24;
const int CODEC = CV_FOURCC('M', 'J', 'P', 'G');

int main()
{
    VideoCapture cap(1); // open the external camera

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
