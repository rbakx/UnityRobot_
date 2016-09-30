#include "RobotDetection.h"

using namespace cv;
using namespace std;

void RobotDetection::processImage()
{
	Mat result = currentFrame.clone();
    //Mat descriptors;

	//orb->compute(currentFrame, sampleKeypoints[3], descriptors);

    

//
//	if(rect.width >= rect.height)
//		circle(result, Point(rect.x + rect.height / 2, rect.y + rect.height / 2), 10, Scalar(0, 0, 255), 20);
//	else
//		circle(result, Point(rect.x + rect.width / 2, rect.y + rect.width / 2), 10, Scalar(0, 0, 255), 20);

    //Mat result = currentFrame.clone();
    //drawKeypoints(result, sampleKeypoints[3], result, Scalar(0, 255, 0));

    vector<DMatch> matches;

    matcher->match(descriptors, currentFrame, matches);


	imshow("Result", descriptors[0]);
}

void RobotDetection::updateRobotPosition(int x, int y)
{

}


RobotDetection::RobotDetection()
{
    //orb = ORB::create();

    FileStorage fs2("keypoints.yml", FileStorage::READ);

    int i = 0;
    while(true) {
        stringstream key;
        key << "Descriptor " << i;

        FileNode fileNode = fs2[key.str()];

        if(fileNode.isNone()) //Reached end of file, we can break out of the loop
            break;

        //vector<KeyPoint> sample;
        // read(fileNode, sample);
        Mat descriptor;
        read(fileNode, descriptor);

        descriptors.push_back(descriptor);

        i++;
    }

    fs2.release();

    cout << "Read " << i << " samples from file" << endl;

    matcher = BFMatcher::create("BruteForce-Hamming");
    matcher->add(descriptors);
}

void RobotDetection::passNewFrame(const Mat& frame)
{
    bufferFrame = currentFrame.clone();
    currentFrame = frame.clone();

    if(bufferFrame.empty())
        return;

	processImage();
}

const vector<Robot>& RobotDetection::getRobots() const
{
    return robots;
}
