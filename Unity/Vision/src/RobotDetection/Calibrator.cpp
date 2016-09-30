#include "Calibrator.h"
#include "../Recorder.h"
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>

using namespace std;
using namespace cv;

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


Calibrator::Calibrator(const string inputFilePath)
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
        findROIs();

        if(waitKey(24) == 27) //Display images in 30fps and when ASCII key 27 (ESC) is pressed, quit application
            break;
    }

    orb->detect(ROIs, sampleKeypoints);
    orb->compute(ROIs, sampleKeypoints, descriptors);

    cout << "Finished calibrating." << endl;
}

Calibrator::Calibrator(Recorder recorder)
: recorder(&recorder)
{
    throw logic_error("Not implemented"); //TODO: Implement this
}

void Calibrator::findROIs()
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

    Rect rect = boundingRect(filteredImage);

    Mat ROI = Mat(currentFrame, rect);
    ROIs.push_back(ROI.clone());
}

void Calibrator::writeToFile(const string filePath) const
{
    FileStorage fs("keypoints.yml", FileStorage::WRITE);
    for(int i = 0; i < descriptors.size(); i++) {
        stringstream key;
        key << "Descriptor " << i;

        //write(fs, key.str(), sampleKeypoints[i]);
        write(fs, key.str(), descriptors[i]);
    }
    fs.release();

    //
    // orb->save(filePath);
}