#ifdef __linux__
	#include <unistd.h> /* For getuid() */
	#include <X11/Xlib.h>
#endif
#include <iostream> /* For ofstream */
#include <fstream>  /* For ofstream */

#include <memory>

#include <thread>


#include "src/framefeeders/CameraFeedSender.hpp"
#include "src/framefeeders/MediaFeedSender.hpp"

#include "src/frames/VideoFeedFrameReceiverTargets.hpp"

#include "src/framereaders/VideoFrameSaver.hpp"
#include "src/framereaders/VideoFrameDisplayer.hpp"
#include "src/framereaders/robotmapping/Calibrator.hpp"

#include "src/framereaders/robotmapping/ShapeDetectorBase.hpp"

#include "src/framereaders/robotmapping/ShapesTracker.hpp"

#include "src/framereaders/robotmapping/MappingSubscriberConsolePrinter.hpp"

#include <communicator.h>
#include <IDataStreamReceiver.hpp>
#include <IPresentationProtocol.hpp>
#include <ProtobufPresentation.h>
#include "src/commlib/MessageBuilder.h"
#include "src/commlib/RobotLogger.h"
#include "src/commlib/communicator.h"
#include "src/MessageHandler.h"
#include <array>


using namespace std;
using namespace frames;
using namespace framefeeders;
using namespace framereaders;
using namespace robotmapping;
using namespace UnityRobot;
using namespace Networking;

void processCommandLineArguments(int argc, char* argv[]);
void printHelp(const string& executablePath);
void configure();

unique_ptr<CameraFeedSender> videofeeder;
unique_ptr<MediaFeedSender> mediafeeder;

VideoFeedFrameReceiverTargets receivers;
unique_ptr<ShapesTracker> tracker;

//TODO remove this
using Msg = Communication::Message;
using MsgBuilder = MessageBuilder;
unique_ptr<TCPSocketDataLink> con;
unique_ptr<Communicator> communicator;
void sendDummyData()
{
	Msg toSend = MsgBuilder::create(Communication::MessageType_::IdentificationResponse, Communication::MessageTarget_::Unity, 13);
	MsgBuilder::addStringData(toSend, "stupid robot");
	MsgBuilder::addNewShape(toSend, 3, { { 5.0, 5.5, 1.0 },{ 1.0, 1.5, 1.5 } });
	MsgBuilder::addNewShape(toSend, 5, { { -5.0, -5.5, -1.0 } });
	MsgBuilder::addChangedShape(toSend, 7, MsgBuilder::createVec3(0, 4, 3));
	MsgBuilder::addDelShape(toSend, 5);

	std::cout << "Send result: " << std::boolalpha << communicator->sendCommand(toSend) << '\n';
}
void sendDummyData2()
{
	Msg toSend2 = MsgBuilder::create(Communication::MessageType_::IdentificationResponse, Communication::MessageTarget_::Unity, 14);
	MsgBuilder::addStringData(toSend2, "stupid robot again");
	MsgBuilder::addNewShape(toSend2, 8, { { 4.0, 5.5, 1.0 },{ 1.0, 1.5, 1.5 } });
	MsgBuilder::addNewShape(toSend2, 5, { { -4.0, -5.5, -1.0 } });
	MsgBuilder::addChangedShape(toSend2, 2, MsgBuilder::createVec3(5, 4, 3));
	MsgBuilder::addDelShape(toSend2, 7);

	std::cout << "Send result: " << std::boolalpha << communicator->sendCommand(toSend2) << '\n';
}

void sendDummyData2(Communicator& com)
{
	Msg toSend2 = MsgBuilder::create(Communication::MessageType_::IdentificationResponse, Communication::MessageTarget_::Unity, 14);
	MsgBuilder::addStringData(toSend2, "stupid robot again");
	MsgBuilder::addNewShape(toSend2, 8, { { 4.0, 5.5, 1.0 },{ 1.0, 1.5, 1.5 } });
	MsgBuilder::addNewShape(toSend2, 5, { { -4.0, -5.5, -1.0 } });
	MsgBuilder::addChangedShape(toSend2, 2, MsgBuilder::createVec3(5, 4, 3));
	MsgBuilder::addDelShape(toSend2, 7);

	std::cout << "Send result: " << std::boolalpha << com.sendCommand(toSend2) << '\n';
}

//TODO remove above
bool initTcpConnection(const std::string& addr, const std::string& port)
{
	con = make_unique<UnityRobot::TCPSocketDataLink>(addr, port, std::unique_ptr<IDataStreamReceiver>(std::make_unique<ProtobufPresentation>()));
	con->Connect();

	if (con->Connected())
	{
		communicator = make_unique<Communicator>(*(static_cast<ProtobufPresentation*>(con->getReceiver())), *con);
		sendDummyData();
		LogInfo("Communicator successfully set up on IP " + addr + ":" + port);
	}
	else
		LogCritError("Communicator failed to set up on IP " + addr + ":" + port);

	return con->Connected();
}

void closeTcpConnection()
{
	//to not rely on destruction order of the logger (TcpSocketDatalink has logging on the destructor)
	this_thread::sleep_for(std::chrono::milliseconds(25)); //wait in case a message is on its way
	communicator.reset(nullptr);
	con.reset(nullptr);
}

int main(int argc, char* argv[])
{
#ifdef __linux__
	XInitThreads(); //Qt needs to know we will be using a multi-threaded environment.
#endif
	RobotLogger::init();

	//GOOGLE_PROTOBUF_VERIFY_VERSION;
	//settings = Settings::read();
	std::string address("145.93.44.194");
	std::string port("1234");
	initTcpConnection(address, port);
	MessageHandler msgHandler(std::move(communicator));
	//sendDummyData2();
	//sendDummyData2();
	
	
	//if(!initTcpConnection(settings->getGeneralProperties().ip, settings->getGeneralProperties().port))
	//	cout << "Connection could not be made to Unity on IP: " << settings->getGeneralProperties().ip
	//		<< ":" << settings->getGeneralProperties().port << endl;

	//processCommandLineArguments(argc, argv);

	//receivers.removeAll();
	videofeeder.reset(nullptr);
	mediafeeder.reset(nullptr);
	tracker.reset(nullptr);
	closeTcpConnection();

	return 0;
}

void processCommandLineArguments(int argc, char* argv[])
{
	if(argc <= 1)
	{	
		printHelp((argc > 0) ? argv[0] : "[unknown executable path]"); //argv[0] contains the relative path from prompt (command line) to executable
		return;

		/*
		 *  POST: No argument is provided, display help and exit.
		 */
	}

	/*
		PRE: the third argument, argv[2] is reserved for filenames for commands. If set, a mediafeeder is always instantiated.
		The program assumes if there is not a third argument, the camera is going to be used
	*/
	if(argc >= 3)
	{
		mediafeeder = make_unique<MediaFeedSender>(&receivers, argv[2]);
		
		/*
		 *  POST: If argument + file path is provided, we will load the file path as a media file feed.
		 */
	}
	else
	{
		videofeeder = make_unique<CameraFeedSender>(&receivers);

		/*
		 *	POST: If only an argument such as 'start' is supplied through command line,
		 *	then start detecting from camera feed.
		 */
	}

	if(strcmp(argv[1], "start") == 0)
	{
		MappingSubscriberConsolePrinter printer;
		
		tracker = make_unique<ShapesTracker>(&printer);
		
		// Scope for display an detectors
		{
			shared_ptr<VideoFrameDisplayer> display = make_shared<VideoFrameDisplayer>();
			receivers.add(display);
			
			vector<shared_ptr<VideoFeedFrameReceiver>> detectors = ShapeDetectorBase::createAndStartDetectorsFromSettings(*tracker);
			receivers.add(detectors);
		}

		cout << "Press enter to stop detecting" << endl;
		cin.ignore(1);
		
		// Deletes all shared_pointers from receivers.
		receivers.removeAll();
	}
	else if(strcmp(argv[1], "calibrate") == 0)
	{
		//Scope for display
		{
			shared_ptr<VideoFrameDisplayer> display = make_shared<VideoFrameDisplayer>();
			receivers.add(display);
		}
		
		//scope for calibrator pointer
		{
			shared_ptr<Calibrator> calibrator = make_shared<Calibrator>();			
			receivers.add(calibrator);

			cout << "Press enter to start calibrating" << endl;
			cin.ignore(1);
			calibrator->Start();

			cout << "Press enter to stop calibrating" << endl;
			cin.ignore(1);
			calibrator->Stop();

			string filePath = "";
			
			while(filePath.length() <= 0)
			{
				cout << "Please provide a file name for storing callibration results:" << endl;
				getline(cin, filePath);
			}
			
			filePath.append(".yml");

			//TODO: Find way to read status of writing
			/*if(calibrator.writeToFile(filePath))
			{
				callibration_incomplete = true;
			}*/

			calibrator->WriteToFile(filePath);
		}

		// Deletes all shared_pointers from receivers.
		receivers.removeAll();
	}
	else if(strcmp(argv[1], "record") == 0)
	{
		//Scope of videosaver
		{
			shared_ptr<VideoFrameSaver> videosaver = make_shared<VideoFrameSaver>();
			receivers.add(videosaver);

			string fileName = "";
			
			while(fileName.length() <= 0)
			{
				cout << "Please provide a file name where you want to store recording:" << endl;
				getline(cin, fileName);
			}
			
			fileName.append(".avi");


			cout << "Press enter to start recording" << endl;
			cin.ignore(1);
			
			videosaver->StartSaving(fileName);

			cout << "Press enter to stop recording" << endl;
			cin.ignore(1);
			
			videosaver->StopSaving();
			
		}
		
		/*
			POST: Start the video recorder and save after recording
		*/
		
		receivers.removeAll();
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
    cout << "  [no arguments]	Shows this list." << endl;
}

void configure()
{
	//TODO: Make this use the config.yml's pid and vid

    #ifdef __linux__
        if(getuid())
		{
            cerr << "You must run this command as a root user as we need to write to /etc/udev/rules.d/" << endl;
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
