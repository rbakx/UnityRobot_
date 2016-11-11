set(CMAKE_C_COMPILER gcc)
set(CMAKE_CXX_COMPILER g++)

cmake_minimum_required(VERSION 3.3)

project(ev3-broker)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

include_directories(asio-1.10.6/include)
include_directories(include)

set(SOURCE_FILES 
	src/main.cpp
	src/ev3wifi.cpp
)

add_executable(ev3-broker ${SOURCE_FILES})
target_link_libraries( ev3-broker )
