#pragma once
#include <string>

namespace UnityRobot {

class LogFileCreator
{
public:
	static std::string createLogFile();
private:
	static std::string createFileNameByDatetime();
};

}
