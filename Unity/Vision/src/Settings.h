#pragma once

#include <opencv2/core/core.hpp>
#include <string>

struct GeneralProperties
{
	std::string ip;
	std::string port;
	std::vector<std::string> sampleNames;

	GeneralProperties() : ip(""), port(""), sampleNames({}) {}
	GeneralProperties(std::string ip, std::string port, std::vector<std::string> sampleNames)
			: ip(ip), port(port), sampleNames(sampleNames) {}
};

struct DeviceProperties
{
    int number;
    int vid;
    int pid;

    DeviceProperties() : number(-1), vid(-1), pid(-1) {}
    DeviceProperties(int number, int vid, int pid) : number(number), vid(vid), pid(pid) {}
};

struct RecordingProperties
{
    int width;
    int height;
    int fps;
	bool autofocus;

    RecordingProperties() : width(-1), height(-1), fps(-1), autofocus(true) {}
    RecordingProperties(int width, int height, int fps, bool autofocus) : width(width), height(height), fps(fps), autofocus(autofocus) {}
};

/*
	PRE: Settings class holds various settings that are used by the Recorder.
*/
class Settings
{
	private:
		Settings();
		GeneralProperties gp;
		DeviceProperties dp;
		RecordingProperties rp;
		std::string filePath;

		void obtainExecutableDirectory();

	public:
		Settings(GeneralProperties gp, DeviceProperties dp, RecordingProperties rp);
		void write(const std::string fileName) const;
		static Settings* read();

		/*
		 * Getters
		 */
		const GeneralProperties& getGeneralProperties() const;
		const DeviceProperties& getDeviceProperties() const;
		const RecordingProperties& getRecordingProperties() const;
		const std::string& getFilePath() const;
};

extern Settings* settings;