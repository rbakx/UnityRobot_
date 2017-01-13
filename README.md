# UnityRobot
## Compiling the proto files for C++
In order to compile the .proto files for C++ you need to run protoc.exe. Once the protobuf libraries are compiled, go to the folder with the binaries and compile by running the following in the command line:

`protoc --proto_path=YOUR_PROTOFILES_DIR --cpp_out=YOUR_OUTPUT_DIR file1.proto file2.proto etc`

`proto_path` is where the proto files are located. It is optional, especially if the proto files are located in the same folder.

`cpp_out` is where the generated files will be placed. It has to be an already existing folder.

Then you just type in the filenames of the proto files, you may add multiple ones like in the example above.

For more information see https://developers.google.com/protocol-buffers/docs/reference/cpp-generated
