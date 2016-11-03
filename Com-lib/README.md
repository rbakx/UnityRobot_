# Communication Library (CMake)
This branch is a (potential outdated) version of the Communication Library, which focusses on adding CMake support to the library instead of solely relying on Visual Studio.

This README is built-up in the following way:
- [Installation](#installation)
  - [Requirements](#requirements)
  - [Walkthrough](#walkthrough)
- [Running](#running)

## Installation

#### Requirements
- Visual Studio or CMake
- [Google Test 1.8.0](https://github.com/google/googletest/archive/release-1.8.0.zip)
- [Asio (non-boost) 1.10.6](https://sourceforge.net/projects/asio/files/asio/1.10.6%20%28Stable%29/asio-1.10.6.zip/download)

#### Walkthrough
We will assume you are capable enough to install Visual Studio and/or CMake.
The project was written for cross-platform compilation, so you have a choice!
###### Windows
To install the Google Test Framework, follow these steps:
1.  Download the Google Test framework archive
2.  Extract to `C:\gtest`
3.  Build the Framework Libraries
4.  Open `C:\gtest\msvc\gtest.sln` in Visual Studio
5.  Set Configuration to "Debug"
6.  Build Solution

We will now install Asio:
1.  Download the Asio archive
2.  Extract it in, for example, `C:\asio`
3.  Now [create the user environment variable](http://www.computerhope.com/issues/ch000549.htm) `ASIO_DIR` and point to the `C:\asio\include` folder
4.  If you have an instance of Visual Studio already running, restart Visual Studio in order to reload the user environment variables.

###### Linux
To install the Google Test Framework, follow these steps:
1.  Download the Google Test Framework archive
2.  Extract it anywhere you want
3.  Run `sudo apt-get install autoconf automake libtool curl make g++ unzip` to install the required tools
3.  Use `cmake` to generate the Makefile
4.  Run `make` and afterwards run `sudo make install`

We will now install Asio:
1.  Download the Asio archive
2.  Extract it anywhere you want
3.  Run `./configure` in this folder
4.  Now run `make` and afterwards run `sudo make install`

Finally, run `sudo ldconfig` to refresh shared library cache.

## Running
The most important thing when starting to use this library, is to make sure you pass your IP address as parameter to your built executable. **Note:** This cannot be your local IP address (localhost, 127.0.0.1), but should be your external IP address!

Another option, albeit not recommended, is to change the hard-coded IP address in the source code (main.cpp) to your own IP address.

*The rest of the tutorial follows.*
