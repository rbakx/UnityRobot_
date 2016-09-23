#pragma once
#include <memory>
#include <spdlog/logger.h>
#include <spdlog/details/spdlog_impl.h>

#define LogInfo(logText)\
do\
{\
	auto _logger= spdlog::get("basic_logger");\
	std::stringstream strss;\
	strss << "\t" << logText << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	_logger->info(strss.str());\
}while (false);

#define LogError(logText)\
do\
{\
	auto _logger= spdlog::get("basic_logger");\
	std::stringstream strss;\
	strss << "\t" << logText << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	_logger->error(strss.str());\
}while (false);

#define LogWarning(logText)\
do\
{\
	auto _logger= spdlog::get("basic_logger");\
	std::stringstream strss;\
	strss << "\t" << logText << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	_logger->warn(strss.str());\
}while (false);

#define LogDebug(logText)\
do\
{\
	auto _logger= spdlog::get("basic_logger");\
	std::stringstream strss;\
	strss << "\t" << logText << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	_logger->debug(strss.str());\
}while (false);

#define LogCritError(logText)\
do\
{\
	auto _logger= spdlog::get("basic_logger");\
	std::stringstream strss;\
	strss << "\t" << logText << "\n\t_FILE: " << __FILE__ << "\n\t_LINE: " << __LINE__;\
	_logger->critical(strss.str());\
}while (false);

namespace UnityRobot {
class RobotLogger
{
public:
	void init();
	void setLevel(spdlog::level::level_enum l) const;
	void setPattern(const std::string& pattern) const;
	void setFlushThreshold(spdlog::level::level_enum l) const;
private:
	std::shared_ptr<spdlog::logger> m_logger;
};

}
