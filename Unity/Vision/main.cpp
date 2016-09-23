#include "Recorder.h"
#include "Display.h"
#include <unistd.h> /* For getuid() */
#include <iostream> /* For ofstream */
#include <fstream>  /* For ofstream */


using namespace std;

string exec_path;

void printHelp();
void configure();

int main(int argc, char* argv[])
{
    exec_path = argv[0];

    if(argc == 1) {
        Display display;
        display.run();
    }
    else {
        if(strcmp(argv[1], "start") == 0) {
            Display display;
            display.run();
        }
        else if(strcmp(argv[1], "record") == 0) {
            Recorder recorder;
            recorder.run();
        }
        else if(strcmp(argv[1], "configure") == 0) {
            configure();
        }
        else {
            printHelp();
        }
    }

    return 0;
}

void printHelp()
{
    cout << "Usage: " << exec_path << " [COMMAND]" << endl;
    cout << endl;
    cout << "Recognises robots and passes the location to Unity3D. Next to that, it attempts to map the world using a 2D top-down image." << endl;
    cout << endl;
    cout << "Commands:" << endl;
    cout << "  start      Starts the robot recognition. This command can be omitted." << endl;
    cout << "  record     Writes camera output to an .avi file with autofocus disabled, useful to create sample data." << endl;
    cout << "  configure  Will write a file to /etc/udev/rules.d/ in order to obtain write access to the webcam." << endl;
    cout << "  help       Shows this list." << endl;
}

void configure()
{
    #ifdef __linux__
        if(getuid()) {
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