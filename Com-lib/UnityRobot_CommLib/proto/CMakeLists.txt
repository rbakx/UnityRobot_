cmake_minimum_required(VERSION 3.3)
project(proto)

add_library (proto SHARED
                    message.pb.cc
                    message.pb.h
                    messages.pb.cc
                    messages.pb.h
                    transform.pb.cc
                    transform.pb.h
            )

set_target_properties(proto PROPERTIES LINKER_LANGUAGE CXX)

# Make sure the compiler can find include files for our proto library
# when other libraries or executables link to proto
target_include_directories (proto PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
