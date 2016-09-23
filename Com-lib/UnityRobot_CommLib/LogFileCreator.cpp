#include "LogFileCreator.h"
#include <chrono>

namespace UnityRobot {

using clock = std::chrono::system_clock;
const std::string logPrefix("Log_");
const std::string logPostfix(".txt");

std::string LogFileCreator::createLogFileName()
{
	return logPrefix + createFileNameByDatetime() + logPostfix;
}

std::string LogFileCreator::createFileNameByDatetime()
{
	time_t now = clock::to_time_t(clock::now());
	char buffer[20];
	tm t = {0};
	localtime_s(&t, &now);
	strftime(buffer, 20, "%Y%b%d_%H%M%S", &t);

	return buffer;
}

}
