#pragma once
#include <string>

namespace UnityRobot {

class LogFileCreator
{
public:
	static std::string createLogFileName();
private:
	static std::string createFileNameByDatetime();
};

}
