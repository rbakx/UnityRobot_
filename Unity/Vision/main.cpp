#include <stdio.h>
#include <opencv2/opencv.hpp>

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

    resizeWindow("Original", 640, 350);
    resizeWindow("Differences", 640, 350);

    VideoCapture cap(CAMERA_FILE_PATH); // open the demo video

    if(!cap.isOpened())  // check if we succeeded
    {
        cout << "Cannot find file" << endl;
        return -1;
    }

    Mat bufferFrame;
    Mat currentFrame;

    while(true)
    {
        if(!currentFrame.empty())
            bufferFrame = currentFrame.clone();

        cap >> currentFrame;

        if(!currentFrame.data)
        {
            cout << "Reached end of file, stopping!" << endl;
            break;
        }


        imshow("Original", currentFrame); //show original

        if(!bufferFrame.empty())
        {
            Mat diffImage;
            //diffImage = Mat::zeros(currentFrame.size(), currentFrame.type());
            absdiff(bufferFrame, currentFrame, diffImage);

            imshow("Differences", diffImage);
        }


        if (waitKey(24) == 27) //Display images in 30fps and when ASCII key 27 (ESC) is pressed, quit application
            break;
    }

    return 0;
}
