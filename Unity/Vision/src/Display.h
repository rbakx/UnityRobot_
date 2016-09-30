#ifndef ASSIGNMENT_DISPLAY_H
#define ASSIGNMENT_DISPLAY_H
#include <string>

class Display {
private:
    const std::string CAMERA_FILE_PATH = "resources/sample.avi";

public:
    Display();
    int run();
};


#endif //ASSIGNMENT_DISPLAY_H
