#include <stdio.h>
#include <opencv2/opencv.hpp>

#include "src/RobotDetection/RobotDetection.h"

using namespace cv;
using namespace std;

const String CAMERA_FILE_PATH = "resources/result2.avi";

int main()
{
    /*
     * Initialize windows
     */
    cvNamedWindow("Original", WINDOW_NORMAL);
    cvNamedWindow("Differences", WINDOW_NORMAL);
    cvNamedWindow("Filtered", WINDOW_NORMAL);
    cvNamedWindow("Result", WINDOW_NORMAL);

    resizeWindow("Original", 640, 350);
    resizeWindow("Differences", 640, 350);
    resizeWindow("Filtered", 640, 350);
	resizeWindow("Result", 640, 350);

    VideoCapture cap(CAMERA_FILE_PATH); // open the demo video

    if(!cap.isOpened())  // check if we succeeded
    {
        cout << "Cannot find file" << endl;
        return -1;
    }

    RobotDetection rd;
    Mat frame;

    while(true)
    {
        cap >> frame;

        if(!frame.data)
        {
            cout << "Reached end of file, stopping!" << endl;
            break;
        }

        imshow("Original", frame); //show original
        rd.passNewFrame(frame);

        if (waitKey(24) == 27) //Display images in 30fps and when ASCII key 27 (ESC) is pressed, quit application
            break;
    }

    return 0;
}
