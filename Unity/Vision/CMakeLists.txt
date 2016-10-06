set(CMAKE_C_COMPILER gcc)
set(CMAKE_CXX_COMPILER g++)

cmake_minimum_required(VERSION 3.3)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/build)
#file(COPY resources DESTINATION build)

add_custom_command(OUTPUT ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/resources
        COMMAND ${CMAKE_COMMAND} -E copy_directory ${CMAKE_SOURCE_DIR}/resources ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/resources)
add_custom_target(copy_dir_target ALL
        DEPENDS ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/resources)


project(Assignment)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

find_package( OpenCV REQUIRED )
find_package( libuvc REQUIRED )
#include_directories(${libuvc_INCLUDE_DIRS})

find_package(PkgConfig REQUIRED)
pkg_search_module(LIBUSB1 REQUIRED libusb-1.0)
include_directories(SYSTEM ${LIBUSB1_INCLUDE_DIRS})

set (SOURCE_FILES   main.cpp
                    src/Robot.h
                    src/RobotDetection/RobotDetection.cpp
                    src/RobotDetection/RobotDetection.h
                    src/Recorder.cpp src/Recorder.h
                    src/Display.cpp src/Display.h
                    src/RobotDetection/Calibrator.cpp src/RobotDetection/Calibrator.h
                    src/Settings.cpp src/Settings.h )
add_executable(Assignment ${SOURCE_FILES})
target_link_libraries( Assignment ${OpenCV_LIBS} uvc usb-1.0)
