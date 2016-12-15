#ifdef __linux__
	#include <unistd.h> /* For getuid() */
#endif
#include <iostream> /* For ofstream */
#include <fstream>  /* For ofstream */

#include "src/framefeeders/CameraFeedSender.hpp"
#include "src/framefeeders/MediaFeedSender.hpp"

#include "src/frames/VideoFeedFrameReceiverTargets.hpp"

#include "src/framereaders/VideoFrameSaver.hpp"
#include "src/framereaders/VideoFrameDisplayer.hpp"
#include "src/framereaders/robotmapping/Calibrator.hpp"
#include "src/framereaders/robotmapping/Detector.hpp"


using namespace std;
using namespace frames;
using namespace framefeeders;
using namespace framereaders;
using namespace robotmapping;

void processCommandLineArguments(int argc, char* argv[]);
void printHelp(const string& executablePath);
void configure();

CameraFeedSender* videofeeder = nullptr;
MediaFeedSender* mediafeeder = nullptr;
VideoFeedFrameReceiverTargets receivers;


int main(int argc, char* argv[])
{
	settings = Settings::read();

	processCommandLineArguments(argc, argv);

	delete videofeeder;
	delete mediafeeder;

	return 0;
}

void processCommandLineArguments(int argc, char* argv[])
{
	if(argc == 1)
	{
		printHelp(argv[0]); //argv[0] contains the relative path from prompt (command line) to executable
		return;

		/*
		 *  POST: No argument is provided, display help and exit.
		 */
	}

	if(argc >= 3)
	{
		mediafeeder = new MediaFeedSender(&receivers, argv[2]);

		/*
		 *  POST: If argument + file path is provided, we will load the file path as a media file feed.
		 */
	}
	else
	{
		videofeeder = new CameraFeedSender(&receivers);

		/*
		 *	POST: If only an argument such as 'start' is supplied through command line,
		 *	then start detecting from camera feed.
		 */
	}

	if(strcmp(argv[1], "start") == 0)
	{
		//Starts all detectors
		vector<VideoFeedFrameReceiver*> detectors = Detector::createReceiversFromSettings();
		receivers.add(detectors);

		VideoFrameDisplayer display;
		receivers.add(&display);
		display.Start();

		cout << "Press enter to stop detecting" << endl;
		cin.ignore(1);

		//Stops all detectors and remove the displayer
		receivers.remove(detectors);
		receivers.remove(&display);

		display.Stop();
	}
	else if(strcmp(argv[1], "calibrate") == 0)
	{
		Calibrator calibrator;
		receivers.add(&calibrator);

		VideoFrameDisplayer displayer;
		receivers.add(&displayer);

		cout << "Press enter to start calibrating" << endl;
		cin.ignore(1);
		calibrator.Start();

		cout << "Press enter to stop calibrating" << endl;
		cin.ignore(1);
		calibrator.Stop();

		string filePath = "";
		cout << "Please provide a file name where you want to store the calibration results:" << endl;
		getline(cin, filePath);

		if(filePath.length() > 0)
		{
			filePath.append(".yml");

			//TODO: Find way to read status of writing
			/*if(calibrator.writeToFile(filePath))
			{
				callibration_incomplete = true;
			}*/

			calibrator.WriteToFile(filePath);
		}

		receivers.remove(&calibrator);
		receivers.remove(&displayer);
	}
	else if(strcmp(argv[1], "record") == 0)
	{
		VideoFrameSaver videosaver;
		receivers.add(&videosaver);

		string fileName = "";
		cout << "Please provide a file name where you want to store recording:" << endl;
		getline(cin, fileName);
		fileName += ".avi";


		cout << "Press enter to start recording" << endl;
		cin.ignore(1);
		videosaver.StartSaving(fileName);

		cout << "Press enter to stop recording" << endl;
		cin.ignore(1);
		videosaver.StopSaving();
		
		/*
			POST: Start the video recorder and save after recording
		*/
	}
	else if(strcmp(argv[1], "configure") == 0)
	{
		configure();
	}
	else
	{
		printHelp(argv[0]); //argv[0] contains the relative path from prompt (command line) to executable
	}
}

void printHelp(const string& executablePath)
{
    cout << "Usage: " << executablePath << " [COMMAND]" << endl;
    cout << endl;
    cout << "Recognises robots and passes the location to Unity3D. Next to that, it attempts to map the world using a 2D top-down image." << endl;
    cout << endl;
    cout << "Commands:\t\t(note: case sensitive!)" << endl;
	cout << "  start      		Starts using camera feed as information source." << endl;
	cout << "  start [file]		Starts playing a video file as information source." << endl;
    cout << "  record     		Writes camera output to an .avi file with autofocus disabled, useful to create sample data." << endl;
    cout << "  calibrate  		Extracts features and stores them to a binary file to be used in the main program." << endl; //TODO: Better description
    cout << "  configure  		Will write a file to /etc/udev/rules.d/ in order to obtain write access to the webcam." << endl;
    cout << "  help       		Shows this list." << endl;
}

void configure()
{
	//TODO: Make this use the config.yml's pid and vid

    #ifdef __linux__
        if(getuid())
		{
            cout << "You must run this command as a root user as we need to write to /etc/udev/rules.d/" << endl;
            return;
        }

        ofstream ruleFile;
        ruleFile.open("/etc/udev/rules.d/UnityRobot.rules");

        ruleFile << "SUBSYSTEM==\"usb\", ATTRS{idVendor}==\"046d\", ATTRS{idProduct}==\"0843\", MODE=\"0666\"\n";
        ruleFile << "SUBSYSTEM==\"usb_device\", ATTRS{idVendor}==\"046d\", ATTRS{idProduct}==\"0843\", MODE=\"0666\"\n";

        ruleFile.close();

        system("sudo /etc/init.d/udev restart");

    #elif __WIN32
        //TODO: Find if permission error occurs on Windows as well
        cout << "Configuring udev rules should not be required in Windows." << endl;
    #endif
}
