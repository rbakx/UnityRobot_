#include "Calibrator.h"
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>

using namespace std;
using namespace cv;
using namespace robotmapping;

void Calibrator::passNewFrame(const cv::Mat &frame)
{
    bufferFrame = currentFrame.clone();

    //TODO: Equalize Histogram?
    /*Mat ycrcb;
    cvtColor(frame, ycrcb, CV_BGR2YCrCb);

    vector<Mat> channels;
    split(ycrcb,channels);

    equalizeHist(channels[0], channels[0]);
    merge(channels,ycrcb);

    cvtColor(ycrcb, currentFrame, CV_YCrCb2BGR);*/

    currentFrame = frame.clone();
}


Calibrator::Calibrator(const string& inputFilePath)
: inputFilePath(inputFilePath)
{
    orb = ORB::create(numberOfFeatures);

    VideoCapture cap(inputFilePath); //open the sample footage provided

    if(!cap.isOpened()) { //check if we succeeded
        cout << "Cannot find file " << inputFilePath << endl;
        exit(-1);
    }

    Mat frame;

    while(true) {
        cap >> frame;

        if(!frame.data) {
            cout << "Reached end of file, stopping!" << endl;
            break;
        }

        passNewFrame(frame);
        updateROI();

        if(waitKey(24) == 27) //Display images in 30fps and when ASCII key 27 (ESC) is pressed, quit application
            break;
    }

    orb->detect(currentFrame, keypoints, ROImask);
    orb->compute(currentFrame, keypoints, descriptors);

    cout << "Finished calibrating." << endl;
}

Calibrator::Calibrator(const Recorder& recorder)
: recorder(&recorder)
{
    throw logic_error("Not implemented"); //TODO: Implement this
}

void Calibrator::updateROI()
{
    if(bufferFrame.empty())
        return;

    Mat grayBuffer, grayCurrent;
    cvtColor(bufferFrame, grayBuffer, CV_BGR2GRAY);
    cvtColor(currentFrame, grayCurrent, CV_BGR2GRAY);

    Mat diffImage, thresholdImage;
    absdiff(grayBuffer, grayCurrent, diffImage);
    threshold(diffImage, thresholdImage, 10, 255, CV_THRESH_BINARY); //TODO: Make this adaptive? --> no.

    Mat filteredImage;
    medianBlur(thresholdImage, filteredImage, 5);

    imshow("Differences", thresholdImage);
    imshow("Filtered", filteredImage);

    if(countNonZero(filteredImage) < 1) //Check if there is something moving, otherwise boundingRect will throw an exception
        return;

    if(ROImask.rows <= 0)
        ROImask = Mat::zeros(currentFrame.size(), CV_8U);

    bitwise_or(ROImask, filteredImage, ROImask); //Merge current mask with overall ROImask
}

void Calibrator::writeToFile(const string& fileName) const
{
    FileStorage fs(fileName, FileStorage::WRITE);

    write(fs, "Descriptors", descriptors);
    write(fs, "Keypoints", keypoints);
    write(fs, "Sample", Mat(currentFrame.clone(), boundingRect(ROImask)));

    fs.release();
}