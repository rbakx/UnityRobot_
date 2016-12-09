#include "RobotLogger.h"
#include "LogFileCreator.h"
#include "spdlog/details/spdlog_impl.h"

namespace UnityRobot {
std::shared_ptr<spdlog::logger>	RobotLogger::m_logger;

void RobotLogger::init()
{
	std::string logPath("logs/" + UnityRobot::LogFileCreator::createLogFileName());
	m_logger = spdlog::basic_logger_mt("basic_logger", logPath);
	setPattern("[%l][%Y%b%d %H:%M:%S.%e][ThreadID:%t]\n%v");
}

void RobotLogger::setLevel(spdlog::level::level_enum l)
{
	m_logger->set_level(l);
}

void RobotLogger::setFlushThreshold(spdlog::level::level_enum l)
{
	m_logger->flush_on(l);
}

void RobotLogger::setPattern(const std::string& pattern)
{
	m_logger->set_pattern(pattern);
}

spdlog::logger* RobotLogger::get()
{
	return m_logger.get();
}

}
