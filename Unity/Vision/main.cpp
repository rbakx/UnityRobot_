#include <stdio.h>
#include <opencv2/opencv.hpp>
#include "filters.h"

using namespace cv;
using namespace std;

/** Global variables */
static String imageFile = "./resources/edge_detection.jpg";

static int data_laplacian[3][3] =   {
                                        {0, -1, 0},
                                        {-1, 4, -1},
                                        {0, -1, 0}
                                    };

static int data_gradient[3][3] =   {
                                        {-1, 0, 1},
                                        {-1, 0, 1},
                                        {-1, 0, 1}
                                    };

/*
 * Swap 'data_laplacian' by 'data_gradient' in order to see different kernels at work
 */
Mat kernel(3, 3, CV_8SC4, data_laplacian);

int main()
{
    Mat image;
    image = imread(imageFile, CV_LOAD_IMAGE_GRAYSCALE);

    if(!image.data)
    {
        cout <<  "Could not open or find the image" << endl;
        return -1;
    }

    imshow("Original", image); //show original

    Mat res = filters::convolute(image, kernel);
    imshow("Laplacian", res);


    while(true) {
        if (waitKey(30) == 27) //Display images in 30fps and when ASCII key 27 (ESC) is pressed, quit application
            break;
    }

    return 0;
}
