#ifndef ASSIGNMENT_SETTINGS_H
#define ASSIGNMENT_SETTINGS_H

#include <opencv2/core/core.hpp>
#include <string>

struct GeneralProperties
{
	int port;
	std::string sampleName;

	GeneralProperties() : port(-1), sampleName("") {}
	GeneralProperties(int port, std::string sampleName) : port(port), sampleName(sampleName) {}
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
		Settings() {};
		GeneralProperties gp;
		DeviceProperties dp;
		RecordingProperties rp;

		static std::string getExecutableDirectory();

	public:
		Settings(GeneralProperties gp, DeviceProperties dp, RecordingProperties rp) : gp(gp), dp(dp), rp(rp) {};
		void write(const std::string fileName) const;
		static Settings* read();

		/*
		 * Getters
		 */
		const GeneralProperties& getGeneralProperties() const;
		const DeviceProperties& getDeviceProperties() const;
		const RecordingProperties& getRecordingProperties() const;
};

extern Settings* settings;


#endif //ASSIGNMENT_SETTINGS_H
