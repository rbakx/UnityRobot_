#pragma once
#include <memory>
#include <sstream> //necessary for the macros
#include "spdlog/logger.h"
#include "spdlog/details/spdlog_impl.h"

#define LogInfo(logText)\
do\
{\
	std::stringstream strss;\
	strss << "\t" << std::string(logText) << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	UnityRobot::RobotLogger::get()->info(strss.str());\
}while (false);

#define LogError(logText)\
do\
{\
	std::stringstream strss;\
	strss << "\t" << std::string(logText) << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	UnityRobot::RobotLogger::get()->error(strss.str());\
}while (false);

#define LogWarning(logText)\
do\
{\
	std::stringstream strss;\
	strss << "\t" << std::string(logText) << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	UnityRobot::RobotLogger::get()->warn(strss.str());\
}while (false);

#define LogDebug(logText)\
do\
{\
	std::stringstream strss;\
	strss << "\t" << std::string(logText) << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	UnityRobot::RobotLogger::get()->debug(strss.str());\
}while (false);

#define LogCritError(logText)\
do\
{\
	std::stringstream strss;\
	strss << "\t" << std::string(logText) << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	UnityRobot::RobotLogger::get()->critical(strss.str());\
}while (false);

namespace UnityRobot {
class RobotLogger
{
public:
	static void init();
	static spdlog::logger* get();
	static void setLevel(spdlog::level::level_enum l);
	static void setPattern(const std::string& pattern);
	static void setFlushThreshold(spdlog::level::level_enum l);
private:
	static std::shared_ptr<spdlog::logger> m_logger;
};

}
