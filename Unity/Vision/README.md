# Vision project
- [Installation](#installation)
  - [Requirements](#requirements)
  - [Walkthrough](#walkthrough)
- [Running](#running)
- [Troubleshooting](#troubleshooting)

## Installation
This small tutorial will guide you through the installation of all required software and libraries in order to run the project.
#### Requirements
To run the Vision project, you need the following things installed:
- OpenCV 3.1 (We **need** this version for ORB)
- MinGW 4.9.x (Other versions might work as well, but I only tried this one.)
- CMake 3.6.x (Other versions might work as well, but I only tried this one.)
- libusb 1.0.19
- libuvc v0.0.5
- A text editor of your choice

#### Walkthrough
Before we start, please make sure your list of packages of all repositories is up-to-date by typing:
```bash
sudo apt-get update
```

We will start with installing MinGW. Use the following [tutorial](http://blog.florianwolters.de/educational/2013/11/21/Installing-MinGW/) if you're not familiar with this. MinGW uses a installation manager, so after downloading an executable you are not done. Make sure you install the following packages:
- Package `mingw-developer-toolkit`; Class `bin`
- Package `mingw32-base`; Class `bin`
- Package `mingw32-gcc-g++`; Class `bin`
- Package `mingw32-gcc-g++`; Class `dev`
- Package `msys-base`; Class `bin`

After you installed the packages, add the `bin` folder inside your installation directory to your system's PATH variable. For example: `C:\MinGW\bin`.

Now we will install CMake, [which is easy as cake](https://cmake.org/install/).

Next, we have to install OpenCV. As we will **not** be using Visual Studio for this, we need to build the OpenCV source by ourselves. [This](http://docs.opencv.org/2.4/doc/tutorials/introduction/windows_install/windows_install.html) tutorial guides you through it. It's recommended by me to let it extract somewhere where you can find it easily, such as in `C:\opencv`. Then, browse via the command line to the `sources` folder and type:
```bash
  cmake -G "MinGW Makefiles"
```
When CMake is done, we have a nice Makefile which we will use to compile the OpenCV source. This is going to take a while!
```bash
  mingw64-make
```
When it is finally done, do not forget to append your system's PATH variable with `C:\opencv\sources\bin`!

Lastly, we need to install libuvc. However, we need to install its dependencies first. So we have to install make and install libusb. In order to do that, we first have to make sure we have libudev installed:
```bash
sudo apt-get install libudev-dev
```

Now, download [libusb](https://sourceforge.net/projects/libusb/files/libusb-1.0/libusb-1.0.19/libusb-1.0.19.tar.bz2/download) and extract the archive where you want. Open the command line and navigate to this folder and run the following line:
```bash
./configure; make; sudo make install
```
This will install libusb for you such that we meet the dependencies to install libuvc.

Lastly, we will install [libuvc](https://github.com/ktossell/libuvc/releases/tag/v0.0.5). Download and extract the archive, `cd` to the folder and execude the following commands:
```bash
mkdir build && cd build && cmake .. && make && sudo make install
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
or:
```bash
  mingw64-make
```
And an executable will be generated in your `build` folder. To run this executable, you can use the Windows or Linux way accordingly.

## Troubleshooting
A lot of things can go wrong, but hopefully this won't happen to you. Yet I decided to start a list of things that you might run into:

#### You ran `cmake` without the `-G "MinGW Makefiles"` flag
When running CMake, it will generate a file called `CMakeCache.txt` and a folder called `CMakeFiles`. Remove these and then run `cmake -G "MinGW Makefiles"`.

#### You try to re-run cmake, but it doesn't seem to have any effect.
When running CMake, it will generate a file called `CMakeCache.txt` and a folder called `CMakeFiles`. I'm not sure yet, but it might be the case that CMake does not overwrite these and therefore will not generate a new `Makefile`. Remove `CMakeCache.txt` and `CMakeFiles` and then run `cmake -G "MinGW Makefiles"`.
