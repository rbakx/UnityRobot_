#include "Settings.h"

using namespace std;
using namespace cv;

////These write and read functions must be defined for the serialization in FileStorage to work
//static void write(FileStorage& fs, const std::string&, const Settings& x)
//{
//    x.write(fs);
//}
//
//static void read(const FileNode& node, Settings& x, const Settings& default_value = Settings()){
//    if(node.empty())
//        x = default_value;
//    else
//        x.read(node);
//}

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

Settings Settings::read(const string fileName)
{
    Settings settingsObj;

    FileStorage fs(fileName, FileStorage::READ);

    FileNode settingsNode = fs["Settings"];
    FileNode deviceNode = settingsNode["Device"];
    FileNode recordingNode = settingsNode["Recording"];

    if(deviceNode.isNone() || recordingNode.isNone()) //We couldn't find the properties in the file
        throw runtime_error("Settings are incomplete!");

    settingsObj.dp = DeviceProperties(deviceNode["number"], deviceNode["pid"], deviceNode["vid"]);
    settingsObj.rp = RecordingProperties(recordingNode["width"], recordingNode["height"], recordingNode["fps"]);

    fs.release();

    return settingsObj;
}

const DeviceProperties& Settings::getDeviceProperties()
{
    return dp;
}

const RecordingProperties& Settings::getRecordingProperties()
{
    return rp;
}