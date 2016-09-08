set(CMAKE_C_COMPILER gcc)
set(CMAKE_CXX_COMPILER g++)

cmake_minimum_required(VERSION 3.3)

project(Assignment)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")

find_package( OpenCV REQUIRED )

set(SOURCE_FILES main.cpp filters.cpp filters.h)
add_executable(Assignment ${SOURCE_FILES})
target_link_libraries( Assignment ${OpenCV_LIBS} )
