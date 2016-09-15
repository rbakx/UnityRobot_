#include <stdio.h>
#include <opencv2/opencv.hpp>
#include "filters.h"

using namespace cv;
using namespace std;

int main()
{
    VideoCapture cap(1); // open the external camera

    if(!cap.isOpened())  // check if we succeeded
        return -1;

    Mat bufferFrame;
    Mat currentFrame;

    while(true)
    {
        if(!currentFrame.empty())
            bufferFrame = currentFrame.clone();

        cap >> currentFrame;

        if(!currentFrame.data)
        {
          cout << "Missed a frame" << endl;
          continue;
        }

        imshow("Original", currentFrame); //show original

        if(!bufferFrame.empty())
        {
            Mat diffImage;
            //diffImage = Mat::zeros(currentFrame.size(), currentFrame.type());
            absdiff(bufferFrame, currentFrame, diffImage);

            imshow("Difference", diffImage);
        }


        if (waitKey(24) == 27) //Display images in 30fps and when ASCII key 27 (ESC) is pressed, quit application
            break;
    }

    return 0;
}
