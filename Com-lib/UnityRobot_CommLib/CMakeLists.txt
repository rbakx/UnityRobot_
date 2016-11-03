set(CMAKE_C_COMPILER gcc)
set(CMAKE_CXX_COMPILER g++)

cmake_minimum_required(VERSION 3.3)

set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/build)

project(ComLib)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
set(CMAKE_MODULE_PATH ${CMAKE_MODULE_PATH} "${CMAKE_SOURCE_DIR}/cmake/modules" )

enable_testing()
find_package(GTest REQUIRED)
include_directories(${GTEST_INCLUDE_DIRS})

find_package( asio REQUIRED )
include_directories( SYSTEM ${asio_INCLUDE} )

find_package(Protobuf REQUIRED)
include_directories(${PROTOBUF_INCLUDE_DIRS})
include_directories(${CMAKE_CURRENT_BINARY_DIR})

protobuf_generate_cpp(PROTO_SRCS PROTO_HDRS
                        proto/message.proto
                        proto/messages.proto
                        proto/transform.proto
                     )

add_subdirectory(spdlog)
add_subdirectory(src/Networking)

set(SOURCE_FILES    MessageBuilder.cpp MessageBuilder.h
                    RobotLogger.cpp RobotLogger.h
                    TCPSocketDataLink.cpp TCPSocketDataLink.hpp
                    LogFileCreator.cpp LogFileCreator.h
   )

add_executable(ComLib main.cpp ${SOURCE_FILES} ${PROTO_SRCS} ${PROTO_HDRS})

target_link_libraries(  ComLib
                        pthread
                        ${PROTOBUF_LIBRARIES}
                        spdlog
                        ${GTEST_BOTH_LIBRARIES}
                        Networking
                      )
