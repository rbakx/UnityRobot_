#include "LogFileCreator.h"
#include <chrono>

#if defined(__linux__)
	#include <time.h>
#endif

namespace UnityRobot {

using clock = std::chrono::system_clock;
const std::string logPrefix("Log_");
const std::string logPostfix(".txt");

std::string LogFileCreator::createLogFileName()
{
	return logPrefix + createFileNameByDatetime() + logPostfix;
}

//TODO: Did not test this yet for cross-platform compatibility
std::string LogFileCreator::createFileNameByDatetime()
{
	char buffer[20];

	time_t now = clock::to_time_t(clock::now());
	tm t = {0};

	#if defined(__linux__)
		localtime_r(&now, &t);
	#elif defined(_WIN32) || defined(_WIN64)
		localtime_s(&t, &now);
	#endif

	strftime(buffer, 20, "%Y%b%d_%H%M%S", &t);

	return buffer;
}

}
