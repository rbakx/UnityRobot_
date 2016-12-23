#C# Networking library
This library is used in Unity and the robot brokers for communication.

## Dependencies
  - Google protocol buffers (generated C# code). Check the [readme][protocolbuffers] before compiling the networking library.
 
## Usage
When the networking library needs to be referenced from a regular C# (Visual Studio) project, a reference can be added to this project:

  1. In the solution explorer, right click on references and select add reference
  2. Select projects on the left side and click the browse button
  3. Select the networking project
 
## Compiling
When the networking library needs to be referenced from a Unity project it should be compiled to a library:

  1. Open the networking Visual Studio project
  2. Set the target framework to .NET 3.5 to be compatible with mono (Unity uses mono)
    - Right click the networking project in the solution explorer
    - Select properties
    - Select the application tab on the left
    - Under "Target framework:" select ".NET Framework 3.5"
  3. (Optionally select Release in the toolbar to build a release build)
  4. Right click the project under in the solution explorer and select build
  
You will find the resulting dll in bin/{Debug/Release} relative to the project directory.

[protocolbuffers]: ../proto/README.md
