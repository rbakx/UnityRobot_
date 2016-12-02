#include "Settings.h"

//For getting the executable directory
#ifdef _WIN32
	#define WIN32_LEAN_AND_MEAN
	#include <string>
	#include <windows.h>
#elif __linux__
	#include <linux/limits.h>
	#include <unistd.h>
#include <iostream>

#endif

using namespace std;
using namespace cv;

Settings* settings = nullptr;

string Settings::getExecutableDirectory()
{
	string path;

	#ifdef _WIN32
		char result[ MAX_PATH ];
		path = string( result, GetModuleFileName( NULL, result, MAX_PATH ) );
	#elif  __linux__
		char result[ PATH_MAX ];
		ssize_t count = readlink( "/proc/self/exe", result, PATH_MAX );
		path = string( result, (count > 0) ? count : 0 );
	#endif

	unsigned long lastSlash = path.find_last_of('/');
	return path.substr(0, lastSlash + 1);
}


void Settings::write(const string fileName) const
{
	//TODO: Either remove this method, or update contents
	FileStorage fs(fileName, FileStorage::WRITE);

	fs  << "Settings"
		<< "{"
			<< "Device"
			<< "{"
				<< "number" << dp.number
				<< "vid" << dp.vid
				<< "pid" << dp.pid
			<< "}"
			<< "Recording"
			<< "{"
				<< "width" << rp.width
				<< "height" << rp.height
				<< "fps" << rp.fps
				<< "autofocus" << rp.autofocus
			<< "}"
		<< "}";

	fs.release();
}

Settings* Settings::read()
{
	Settings* settingsObj = new Settings();

	string configFileLocation = Settings::getExecutableDirectory() + "config.yml";
	cout << "Reading config file at " << configFileLocation << endl;
	FileStorage fs(configFileLocation, FileStorage::READ);

	FileNode settingsNode = fs["Settings"];
	FileNode generalNode = settingsNode["General"];
	FileNode deviceNode = settingsNode["Device"];
	FileNode recordingNode = settingsNode["Recording"];

	if(generalNode.isNone() || deviceNode.isNone() || recordingNode.isNone()) //We couldn't find the properties in the file
		throw runtime_error("Settings are incomplete!");

	settingsObj->gp = GeneralProperties(generalNode["port"], generalNode["sampleName"]);
	settingsObj->dp = DeviceProperties(deviceNode["number"], deviceNode["pid"], deviceNode["vid"]);

	FileNode autofocusNode = recordingNode["autofocus"];
	settingsObj->rp = RecordingProperties(recordingNode["width"],
										  recordingNode["height"],
										  recordingNode["fps"],
										  autofocusNode.isNone() ? true : static_cast<int>(autofocusNode) == 1
	);

	fs.release();

	return settingsObj;
}

const GeneralProperties& Settings::getGeneralProperties() const
{
	return gp;
}

const DeviceProperties& Settings::getDeviceProperties() const
{
	return dp;
}

const RecordingProperties& Settings::getRecordingProperties() const
{
	return rp;
}