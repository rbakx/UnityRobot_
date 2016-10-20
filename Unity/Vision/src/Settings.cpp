#include "Settings.h"

using namespace std;
using namespace cv;

Settings* settings = nullptr;

void Settings::write(const string fileName) const
{
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
            << "}"
        << "}";

    fs.release();
}

Settings* Settings::read(const string fileName)
{
    Settings* settingsObj = new Settings();

    FileStorage fs(fileName, FileStorage::READ);

    FileNode settingsNode = fs["Settings"];
	FileNode generalNode = settingsNode["General"];
    FileNode deviceNode = settingsNode["Device"];
    FileNode recordingNode = settingsNode["Recording"];

    if(generalNode.isNone() || deviceNode.isNone() || recordingNode.isNone()) //We couldn't find the properties in the file
        throw runtime_error("Settings are incomplete!");

	settingsObj->gp = GeneralProperties(generalNode["port"], generalNode["sampleName"]);
    settingsObj->dp = DeviceProperties(deviceNode["number"], deviceNode["pid"], deviceNode["vid"]);
    settingsObj->rp = RecordingProperties(recordingNode["width"], recordingNode["height"], recordingNode["fps"]);

    fs.release();

    return settingsObj;
}

const GeneralProperties& Settings::getGeneralProperties()
{
    return gp;
}

const DeviceProperties& Settings::getDeviceProperties()
{
    return dp;
}

const RecordingProperties& Settings::getRecordingProperties()
{
    return rp;
}