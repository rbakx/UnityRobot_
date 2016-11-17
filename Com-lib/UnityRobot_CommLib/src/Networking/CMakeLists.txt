add_library (Networking
                IDataLink.hpp
                IDataStreamReceiver.hpp
                IMessageReceiver.hpp
                IMessageSender.hpp
                IPresentationProtocol.hpp
            )

set_target_properties(Networking PROPERTIES LINKER_LANGUAGE CXX)

# Make sure the compiler can find include files for our Networking library
# when other libraries or executables link to Networking
target_include_directories (Networking PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
