#ifndef ASSIGNMENT_SETTINGS_H
#define ASSIGNMENT_SETTINGS_H

#include <opencv2/core/core.hpp>

struct DeviceProperties {
    int number;
    int vid;
    int pid;

    DeviceProperties() : number(-1), vid(-1), pid(-1) {}
    DeviceProperties(int number, int vid, int pid) : number(number), vid(vid), pid(pid) {}
};

struct RecordingProperties {
    int width;
    int height;
    int fps;

    RecordingProperties() : width(-1), height(-1), fps(-1) {}
    RecordingProperties(int width, int height, int fps) : width(width), height(height), fps(fps) {}
};

class Settings {
private:
    Settings() {};
    DeviceProperties dp;
    RecordingProperties rp;

public:
    Settings(DeviceProperties dp, RecordingProperties rp) : dp(dp), rp(rp) {};
    void write(const std::string fileName) const;
    static Settings* read(const std::string fileName);

    /*
     * Getters
     */
    const DeviceProperties& getDeviceProperties();
    const RecordingProperties& getRecordingProperties();
};


#endif //ASSIGNMENT_SETTINGS_H
