//
// Created by rutger on 6-9-16.
//

#include "filters.h"
#include <stdio.h>

using namespace std;

uchar filters::applyKernel(const Mat& image, Mat& kernel, int x, int y)
{
    if(kernel.empty())
    {
        cout << "Kernel is empty." << endl;
        exit(-1);
    }

    if((kernel.rows % 3) || (kernel.cols % 3))
    {
        cout << "Kernel does not have a middle." << endl;
        exit(-1);
    }


//    cout << kernel.at<int>(0, 0) << "," << kernel.at<int>(0, 1) << "," << kernel.at<int>(0, 2) << endl;
//    cout << kernel.at<int>(1, 0) << "," << kernel.at<int>(1, 1) << "," << kernel.at<int>(1, 2) << endl;
//    cout << kernel.at<int>(2, 0) << "," << kernel.at<int>(2, 1) << "," << kernel.at<int>(2, 2) << endl << endl;


    int result = 0;
    int cellsPassed = 0;

    int topLeft_x = x - (kernel.cols / 2);
    int topLeft_y = y - (kernel.rows / 2);

    for(int i = 0; i < kernel.cols; i++)
    {
        for(int j = 0; j < kernel.rows; j++)
        {
            if(topLeft_x + i < 0 || topLeft_y + j < 0 || topLeft_x + i >= image.cols || topLeft_y + j >= image.rows) //Out of bounds checking
                break;


            uchar pixelValue = image.at<uchar>(Point(topLeft_x + i, topLeft_y + j));
            int scaledPixel = pixelValue * kernel.at<int>(Point(i, j));

            result += scaledPixel;

            cellsPassed++;
        }
    }

    if(cellsPassed == 0)
        return 0;

    return result / cellsPassed;
}

Mat filters::convolute(const Mat& image, Mat& kernel)
{
    Mat result(image.rows, image.cols, image.type());

    flip(kernel, kernel, -1); //Rotate kernel 180 degrees

    for(int x = 0; x < image.cols; x++)
    {
        for(int y = 0; y < image.rows; y++)
        {
            uchar pixelValue = applyKernel(image, kernel, x, y);
            result.at<uchar>(Point(x, y)) = pixelValue;
        }
    }

    return result;
}