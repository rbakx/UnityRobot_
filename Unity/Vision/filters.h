//
// Created by rutger on 6-9-16.
//

#ifndef ASSIGNMENT_FILTERS_H
#define ASSIGNMENT_FILTERS_H
#include <opencv2/opencv.hpp>

using namespace cv;

class filters
{
private:
    static uchar applyKernel(const Mat& image, Mat& kernel, int x, int y);

public:
    static Mat convolute(const Mat& image, Mat& kernel);
};


#endif //ASSIGNMENT_FILTERS_H
