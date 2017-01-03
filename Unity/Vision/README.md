# Vision project
- [Installation](#installation)
  - [Requirements](#requirements)
  - [Walkthrough](#walkthrough)
- [Running](#running)
- [Troubleshooting](#troubleshooting)

The vision project is a c++ program that is able to track objects using a media or camera feed. If connected to a TCP host, events are propagated using a [networking library][cpp_network_library] using [Google Protobuffer messages][cpp_proto_messages].

## Installation
This readme is a walkthrough to setup the vision project so it can be run on Windows (recommended) and Linux.
If your system is 64-bit, you can go ahead and install 64-bit versions of all the programs. Be aware: The Vision project is compiled for 32-bit architecture.

### Windows
#### Requirements
To run the Vision project, you need the following things installed:
- OpenCV 3.1 (We **need** this version for ORB)
- MinGW 4.9.x [Other versions might work as well, not-tested]
- CMake 3.6.x [Other versions might work as well, not-tested]
- libusb 1.0.19 [Required for using the camera reliably]
- libuvc v0.0.5 [Required for using the camera reliably]
- Any text editor

##### Walkthrough
We will start with installing MinGW. Use the following [tutorial](http://blog.florianwolters.de/educational/2013/11/21/Installing-MinGW/) if you're not familiar with MinGW installation manager. Make sure you install the following packages:
- Package `mingw-developer-toolkit`; Class `bin`
- Package `mingw32-base`; Class `bin`
- Package `mingw32-gcc-g++`; Class `bin`
- Package `mingw32-gcc-g++`; Class `dev`
- Package `msys-base`; Class `bin`

*Note: mingw64-*\* *Packages can also be installed.*

After you installed the packages, add the `bin` folder inside your installation directory of MinGW to your system's PATH variable. For example: `C:\MinGW\bin`.

To verify MinGW is installed properly, the following command can be entered in command prompt:
```bash
g++ --version
mingw32-make -version
```
Console output:
```bash
g++ (GCC) 5.4.0
Copyright (C) 2015 Free Software Foundation, Inc.
This is free software; see the source for copying conditions.  There is NO
warranty; not even for MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

GNU Make 3.82.90
Built for i686-pc-mingw32
Copyright (C) 1988-2012 Free Software Foundation, Inc.
License GPLv3+: GNU GPL version 3 or later <http://gnu.org/licenses/gpl.html>
This is free software: you are free to change and redistribute it.
There is NO WARRANTY, to the extent permitted by law.
```

Now we will install CMake, [which is easy as cake](https://cmake.org/install/). Be sure to install version 3.6.x or higher.
In command prompt, you can enter the following command to verify a proper installation:
```bash
cmake -version
```
Console output:
```bash
cmake version 3.6.2

CMake suite maintained and supported by Kitware (kitware.com/cmake).
```

Next, we have to install OpenCV. We will not use Visual Studio in this walkthrough. To use Visual Studio, you can follow this [tutorial for Visual Studio](http://docs.opencv.org/2.4/doc/tutorials/introduction/windows_install/windows_install.html#cpptutwindowsmakeown).

When using anything other than Visual Studio, OpenCV needs to be build ourselves.
Use any git client to pull https://github.com/opencv/opencv git reposity.

Tips:
- OpenCV source version 3.1 can be build the same way as OpenCV 2.4.
- MinGW works on Windows versions 8 and 10 also.
- Any git client can be used; [Git Client](https://desktop.github.com/), [GitKraken](https://www.gitkraken.com/download), [TortoiseGit](https://tortoisegit.org/download/).
- Clone the OpenCV git repository to an easily accessable folder, such as `c:\opencv`.

After extraction of the OpenCV Git, browse using the command prompt to `c:\opencv\sources` folder and type:
```bash
  cmake -G "MinGW Makefiles"
```
It takes a while (approximately around 15 minutes) to execute the cmake command. After the command is processed, we have a nice Makefile which we can use to compile the OpenCV source. Compiling can take up to **1 hour and 45 minutes**!
```bash
  mingw32-make
```
When it is finally done, do not forget to append `c:\opencv\sources\bin` to your system's [PATH variable](windows_path_var)!
Appending this path will allow cmake to find the package that is build on default, representing opencv as a cmake package.

You can now build the openCV project files by navigating to the directory of this readme, navigate to './build' and enter command
```bash
  cmake ../ -DUSE_USB_LIBS=OFF
```

Lastly, we need to install libuvc for using camera reliably. Note that libuvc and libusb are not required, but highly recommended. To install libuvc, its dependencies need to be installed first. The main dependency, libusb, needs to be build and installed first.

Download [libusb](https://sourceforge.net/projects/libusb/files/libusb-1.0/libusb-1.0.19/libusb-1.0.19.tar.bz2/download) and extract the archive anywhere, I.E. `c:\libusb`. Open the command prompt:
```bash
cd c:\libusb
./configure
make
sudo make install
```
This will install libusb for you such that we meet the dependencies to install libuvc.

Lastly, we will install [libuvc](https://github.com/ktossell/libuvc/releases/tag/v0.0.5) the same way as libusb. Download and extract the archive to `c:\libuvc`, execute the following commands:
```bash
cd c:\libuvc
mkdir build
cd build
cmake ..
make
make install
```

## Running
To compile and run the project, type:
```bash
cmake -G "MinGW Makefiles"
```
to let CMake generate a `Makefile` for you. You should only need to call this when you never created a `Makefile` before, or you changed the `CMakeList.txt` (because you for example added a file).

When CMake is done, you can type:
```bash
  mingw32-make
```
And an executable will be generated in your `build` folder. To run this executable, you can use the Windows or Linux way accordingly.

## Troubleshooting
A lot of things can go wrong, but hopefully this won't happen to you. Yet I decided to start a list of things that you might run into:

#### You ran `cmake` without the `-G "MinGW Makefiles"` flag
When running CMake, it will generate a file called `CMakeCache.txt` and a folder called `CMakeFiles`. Remove these and then run `cmake -G "MinGW Makefiles"`.

#### You try to re-run cmake, but it doesn't seem to have any effect.
When running CMake, it will generate a file called `CMakeCache.txt` and a folder called `CMakeFiles`. I'm not sure yet, but it might be the case that CMake does not overwrite these and therefore will not generate a new `Makefile`. Remove `CMakeCache.txt` and `CMakeFiles` and then run `cmake -G "MinGW Makefiles"`.

* * *

TO BE TESTED/CONTINUED

### Linux
#### Requirements
To run the Vision project, you need the following things installed:
- OpenCV 3.1 (We **need** this version for ORB)
- CMake 3.6.x (Other versions might work as well, but I only tried this one.)
- libusb 1.0.19 [Required for using the camera reliably]
- libuvc v0.0.5 [Required for using the camera reliably]
- Any text editor

##### Walkthrough
Before we installing, please make sure your list of packages of all repositories is up-to-date by typing:
```bash
sudo apt-get update
```

Lastly, we need to install libuvc for using camera reliably. However, we need to install its dependencies first. The main dependency is libusb, which needs to be install and build. In order to do that, we first have to make sure we have libudev installed:
```bash
sudo apt-get install libudev-dev
```

Lastly, we will install [libuvc](https://github.com/ktossell/libuvc/releases/tag/v0.0.5). Download and extract the archive, `cd` to the folder and execude the following commands:
```bash
mkdir build && cd build && cmake .. && make && sudo make install
```


[windows_path_var]: http://www.computerhope.com/issues/ch000549.htm
[cpp_network_library]: http://example.com
[cpp_proto_messages]: http://example.com
