# Communication Library (CMake)
This branch is a (potential outdated) version of the Communication Library, which focusses on adding CMake support to the library instead of solely relying on Visual Studio.

This README is built-up in the following way:
- [Installation](#installation)
  - [Requirements](#requirements)
  - [Walkthrough](#walkthrough)
    - [Windows](#windows)
    - [Linux](#linux)
- [Running](#running)

## Installation

#### Requirements
- MSVC or CMake
- [Google Test 1.8.0](https://github.com/google/googletest/archive/release-1.8.0.zip)
- [Asio (non-boost) 1.10.6](https://sourceforge.net/projects/asio/files/asio/1.10.6%20%28Stable%29/asio-1.10.6.zip/download)
- [Protocol Buffers 3.0.2](https://github.com/google/protobuf/releases/download/v3.0.2/protobuf-cpp-3.0.2.zip)

#### Walkthrough
We will assume you are capable enough to install MSVC and/or CMake.
The project was written for cross-platform compilation, so you have a choice!
###### Windows
To install the Google Test Framework, follow these steps:

*//TODO: I don't like this copy-paste from Marco's README, but I didn't check this myself yet:*

1.  Download the Google Test framework archive
2.  Extract to `C:\gtest`
3.  Build the Framework Libraries
4.  Open `C:\gtest\msvc\gtest.sln` in Visual Studio
5.  Set Configuration to "Debug"
6.  Build Solution

We will now install Asio:

1.  Download the Asio archive
2.  Extract it in, for example, `C:\asio`
3.  Now [create the user environment variable](http://www.computerhope.com/issues/ch000549.htm) `ASIO_DIR` and point it to the `C:\asio\include` folder

Lastly, we will install Protocol Buffers:

1.  Download the protobuf archive
2.  Extract it in, for example, `C:\protobuf`
3.  Create a folder within `C:\protobuf` called `lib`
4.  Download and extract the archive
([x86](https://drive.google.com/open?id=0B4kc2adgtRMLLTBHZGZaYnc0N28)/[x64](https://drive.google.com/open?id=0ByRP9F3xvboOWkdDdW51aVdlZ1k))
to this `lib` folder. This archive contains pre-compiled libraries we built for you.
3.  Create a user environment variable called `PROTOBUF_DIR` and point it to the `C:\protobuf` folder

If you have an instance of Visual Studio already running, restart Visual Studio in order to reload the user environment variables!

###### Linux
To install the Google Test Framework, follow these steps:

1.  Run `sudo apt-get install autoconf automake libtool curl make g++ unzip` to install the required tools
2.  Download the Google Test Framework archive
3.  Extract it anywhere you want
4.  Use `cmake` to generate the Makefile
5.  Run `make` and afterwards run `sudo make install`

We will now install Asio:

1.  Download the Asio archive
2.  Extract it anywhere you want
3.  Run `./configure --without-boost` in this folder
4.  Now run `make` and afterwards run `sudo make install`

Next, we will install Protocol Buffers:

1.  Run `sudo apt-get install autoconf automake libtool curl make g++ unzip` to install the required tools if you haven't done so already when installing the Google Test Framework.
2.  Download the protobuf archive
3.  Extract it anywhere you want
4.  Run `./configure` in this folder
5.  Go into the `src` folder
5.  Run `make` and afterwards run `sudo make install`
7. Finally, run `sudo ldconfig` to refresh shared library cache.

## Running
The most important thing when starting to use this library, is to make sure you pass your IP address as parameter to your built executable. **Note:** This cannot be your local IP address (localhost, 127.0.0.1), but should be your external IP address!

Another option, albeit not recommended, is to change the hard-coded IP address in the source code (main.cpp) to your own IP address.

*The rest of the tutorial follows.*
