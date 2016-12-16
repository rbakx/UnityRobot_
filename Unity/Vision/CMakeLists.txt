set(CMAKE_C_COMPILER gcc)
set(CMAKE_CXX_COMPILER g++)

cmake_minimum_required(VERSION 3.3)

# Some definitions
## Colors

if(NOT WIN32)
  string(ASCII 27 Esc)
  set(ColourReset "${Esc}[m")
  set(ColourBold  "${Esc}[1m")
  set(Red         "${Esc}[31m")
  set(Green       "${Esc}[32m")
  set(Yellow      "${Esc}[33m")
  set(Blue        "${Esc}[34m")
  set(Magenta     "${Esc}[35m")
  set(Cyan        "${Esc}[36m")
  set(White       "${Esc}[37m")
  set(BoldRed     "${Esc}[1;31m")
  set(BoldGreen   "${Esc}[1;32m")
  set(BoldYellow  "${Esc}[1;33m")
  set(BoldBlue    "${Esc}[1;34m")
  set(BoldMagenta "${Esc}[1;35m")
  set(BoldCyan    "${Esc}[1;36m")
  set(BoldWhite   "${Esc}[1;37m")
endif()

option(USE_USB_LIBS "Enables the definitions and library linkage to libuvc" ON)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR})

# check for polluted source tree
if(EXISTS ${CMAKE_SOURCE_DIR}/CMakeCache.txt OR
EXISTS ${CMAKE_SOURCE_DIR}/CMakeFiles)
message(FATAL_ERROR
"CMakeCache.txt or CMakeFiles exists in source directory!\nPlease remove the file and folder and build out-of-source.")
endif()
# make sure the user doesn't play dirty with symlinks
get_filename_component(srcdir "${CMAKE_SOURCE_DIR}" REALPATH)
get_filename_component(bindir "${CMAKE_RUNTIME_OUTPUT_DIRECTORY}" REALPATH)

# disallow in-source builds
if(${srcdir} STREQUAL ${bindir})
message(FATAL_ERROR "In-source builds are forbidden!\nPlease remove the file and folder and build out-of-source.")
endif()

#file(COPY config.yml DESTINATION ${CMAKE_RUNTIME_OUTPUT_DIRECTORY})

project(Vision)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++14 -Wall -Werror")

find_package( OpenCV REQUIRED )

set(USB_LIBS "")

if(USE_USB_LIBS)

	list(append USB_LIBS uvc usb-1.0)

	add_definitions(-DUSE_USB_LIBRARIES)
	find_package( libuvc REQUIRED )
endif()
#include_directories(${libuvc_INCLUDE_DIRS})

#find_package(PkgConfig REQUIRED)
#pkg_search_module(LIBUSB1 REQUIRED libusb-1.0)
include_directories(SYSTEM ${LIBUSB1_INCLUDE_DIRS})

set (SOURCE_FILES	main.cpp
					src/Robot.h
					src/framereaders/robotmapping/Detector.cpp
					src/framereaders/robotmapping/Detector.hpp
					src/framereaders/robotmapping/Calibrator.cpp src/framereaders/robotmapping/Calibrator.hpp
					src/Settings.cpp src/Settings.h
					src/Runnable.cpp src/Runnable.hpp
					src/framefeeders/CameraFeedSender.cpp src/framefeeders/CameraFeedSender.hpp
					src/frames/VideoFeedFrameReceiver.hpp
					src/frames/VideoFeedFrameReceiverTargets.cpp src/frames/VideoFeedFrameReceiverTargets.hpp
					src/frames/VideoFeedFrameSender.cpp src/frames/VideoFeedFrameSender.hpp
					src/framereaders/VideoFrameSaver.cpp src/framereaders/VideoFrameSaver.hpp
					src/framefeeders/MediaFeedSender.cpp src/framefeeders/MediaFeedSender.hpp src/framereaders/VideoFrameDisplayer.cpp src/framereaders/VideoFrameDisplayer.hpp
					src/framereaders/VideoFrameDisplayer.cpp src/framereaders/VideoFrameDisplayer.hpp
		)

add_executable(Vision ${SOURCE_FILES})
target_link_libraries( Vision ${OpenCV_LIBS} ${USB_LIBS} rt pthread X11)

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
		
		# Using a random key allows for having duplicate file names
		string(RANDOM LENGTH 3 RANDOM_KEY)
		
		string(TOLOWER "${filename}" filename_lower)
		
		# Name insensitive list of filenames that need to be ignored
		if(filename_lower STREQUAL "readme.md")
			continue()
		endif()
		
		set(filename_suffixed "${filename}_${RANDOM_KEY}")
		
		set(path_component "")
				
        while(1)
            get_filename_component(path_component ${dir} NAME)
			
			if(dirname STREQUAL path_component)
				break()
			endif()
			
            set (output "${path_component}/${output}")
            get_filename_component(dir ${dir} DIRECTORY)
        endwhile()

        set(output "${_outputDirectory}/${output}${filename}")
		

        add_custom_command(
                COMMENT "Copying updated resource-file '${filename}'"
                OUTPUT ${output}
                DEPENDS ${resource}
                COMMAND ${CMAKE_COMMAND} -E copy_if_different
                "${resource}"
                "${output}"
        )

        message("${BoldCyan}path: ${resource}${ColourReset}")
		message("${BoldGreen}path output: ${output}${ColourReset}")
        message("${Magenta}target: ${filename_suffixed}${ColourReset}")

        add_custom_target(${filename_suffixed} ALL DEPENDS ${resource} ${output})
        #add_dependencies(${_target} ${filename})

        #add_custom_command(${filename} POST_BUILD DEPENDS ${resource} COMMAND ${output})
        #add_dependencies(${_target} ${filename})
    endforeach()
endmacro(COPY_RESOURCES_ON_TARGET_BUILD)

COPY_RESOURCES_ON_TARGET_BUILD(Vision ${CMAKE_SOURCE_DIR}/resources ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/resources)

COPY_RESOURCES_ON_TARGET_BUILD(Vision ${CMAKE_SOURCE_DIR}/samples ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/samples)

# config.yml MAY not exist!, might want to add a check here so the project prints to console that it is missing
file(COPY ${CMAKE_SOURCE_DIR}/config.yml DESTINATION ${CMAKE_RUNTIME_OUTPUT_DIRECTORY})
message("config.yml is being copied to build directory, but is only copied when cmake is called!")

file(MAKE_DIRECTORY ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/resources)
file(MAKE_DIRECTORY ${CMAKE_RUNTIME_OUTPUT_DIRECTORY}/samples)