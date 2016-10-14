#ifndef ASSIGNMENT_RECORDER_H
#define ASSIGNMENT_RECORDER_H

#include <opencv2/opencv.hpp>
#include "libuvc/libuvc.h"
#include "Settings.h"

/*
	PRE: 
*/
class Recorder
{
	private:
		const std::string DESTINATION = "result.avi";

		//Opens the Logitech C930e which is assumed to be the second connected camera
		//(as most laptops have a webcam as first camera)
		int deviceNumber = 1;
		int vid = 0x046d;
		int pid = 0x0843;

		int width = 1920;
		int height = 1080;
		double fps = 24;
		const int CODEC = CV_FOURCC('M', 'J', 'P', 'G');

		void setAutoFocus();

	public:
		Recorder();
		int run();
};

#endif //ASSIGNMENT_RECORDER_H
