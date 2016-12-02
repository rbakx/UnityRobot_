cmake_minimum_required(VERSION 3.3)
project(spdlog)

add_library (spdlog SHARED
                async_logger.h
                common.h
                formatter.h
                logger.h
                spdlog.h
                tweakme.h
#                details/async_logger_impl.h
#                details/async_log_helper.h
#                details/file_helper.h
#                details/logger_impl.h
#                details/log_msg.h
#                details/mpmc_bounded_q.h
#                details/null_mutex.h
#                details/os.h
#                details/pattern_formatter_impl.h
#                details/registry.h
#                details/spdlog_impl.h
#                fmt/fmt.h
#                fmt/ostr.h
#                fmt/bundled/format.cc
#                fmt/bundled/format.h
#                fmt/bundled/ostream.cc
#                fmt/bundled/ostream.h
#                fmt/bundled/printf.h
#                sinks/android_sink.h
#                sinks/ansicolor_sink.h
#                sinks/base_sink.h
#                sinks/dist_sink.h
#                sinks/file_sinks.h
#                sinks/msvc_sink.h
#                sinks/null_sink.h
#                sinks/ostream_sink.h
#                sinks/sink.h
#                sinks/stdout_sinks.h
#                sinks/syslog_sink.h
            )

include_directories(details fmt sinks)

set_target_properties(spdlog PROPERTIES LINKER_LANGUAGE CXX)

# Make sure the compiler can find include files for our spdlog library
# when other libraries or executables link to spdlog
target_include_directories (spdlog PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
