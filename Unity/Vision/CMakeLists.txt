set(CMAKE_C_COMPILER gcc)
set(CMAKE_CXX_COMPILER g++)

cmake_minimum_required(VERSION 3.3)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/build)

#file(COPY config.yml DESTINATION ${CMAKE_RUNTIME_OUTPUT_DIRECTORY})

project(Vision)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

find_package( OpenCV REQUIRED )
find_package( libuvc REQUIRED )
#include_directories(${libuvc_INCLUDE_DIRS})

find_package(PkgConfig REQUIRED)
pkg_search_module(LIBUSB1 REQUIRED libusb-1.0)
include_directories(SYSTEM ${LIBUSB1_INCLUDE_DIRS})

set (SOURCE_FILES   main.cpp
                    src/Robot.h
                    src/framereaders/robotmapping/Detector.cpp
                    src/framereaders/robotmapping/Detector.hpp
                    src/framereaders/robotmapping/Calibrator.cpp src/framereaders/robotmapping/Calibrator.hpp
                    src/Settings.cpp src/Settings.h
                    src/framefeeders/CameraFeedSender.cpp src/framefeeders/CameraFeedSender.hpp
                    src/frames/VideoFeedFrameReceiver.hpp
                    src/frames/VideoFeedFrameReceiverTargets.cpp src/frames/VideoFeedFrameReceiverTargets.hpp
                    src/frames/VideoFeedFrameSender.cpp src/frames/VideoFeedFrameSender.hpp
                    src/framereaders/VideoFrameSaver.cpp src/framereaders/VideoFrameSaver.hpp
                    src/framefeeders/MediaFeedSender.cpp src/framefeeders/MediaFeedSender.hpp src/framereaders/VideoFrameDisplayer.cpp src/framereaders/VideoFrameDisplayer.hpp
                    src/framereaders/VideoFrameDisplayer.cpp src/framereaders/VideoFrameDisplayer.hpp
        )

add_executable(Vision ${SOURCE_FILES})
target_link_libraries( Vision ${OpenCV_LIBS} uvc usb-1.0 rt pthread)








macro(COPY_RESOURCES_ON_TARGET_BUILD _target _sourceDirectory _outputDirectory)
    # Scan through resource folder for updated files and copy if none existing or changed

    message("Collecting resources for ${_target} in directory ${_sourceDirectory}/")
    message("Will put all resources in folder ${_outputDirectory}/")

    file (GLOB_RECURSE _resources "${_sourceDirectory}/*")

    foreach(resource ${_resources})
        get_filename_component(filename ${resource} NAME)
        get_filename_component(dir ${resource} DIRECTORY)
        get_filename_component(dirname ${dir} NAME)

        set (output "")

        while(NOT ${dirname} STREQUAL resources)
            get_filename_component(path_component ${dir} NAME)
            set (output "${path_component}/${output}")
            get_filename_component(dir ${dir} DIRECTORY)
            get_filename_component(dirname ${dir} NAME)
        endwhile()

        set(output "${_outputDirectory}/${output}/${filename}")

        add_custom_command(
                COMMENT "Moving updated resource-file '${filename}'"
                OUTPUT ${output}
                DEPENDS ${resource}
                COMMAND ${CMAKE_COMMAND} -E copy_if_different
                ${resource}
                ${output}
        )

        message("${resource}")
        message("${filename}")

        add_custom_target(${filename} ALL DEPENDS ${resource} ${output})
        #add_dependencies(${_target} ${filename})

        #add_custom_command(${filename} POST_BUILD DEPENDS ${resource} COMMAND ${output})
        #add_dependencies(${_target} ${filename})
    endforeach()
endmacro(COPY_RESOURCES_ON_TARGET_BUILD)

COPY_RESOURCES_ON_TARGET_BUILD(Vision ${CMAKE_SOURCE_DIR}/resources ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/resources)