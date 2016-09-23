Get Google C++ Testing Framework at https://github.com/google/googletest

Download the latest gtest framework
Unzip to C:\gtest
Build the Framework Libraries

Open C:\gtest\msvc\gtest.sln in Visual Studio
Set Configuration to "Debug"
Build Solution
Create and Configure Your Test Project

Create a new solution and choose the template Visual C++ > Win32 > Win32 Console Application
Right click the newly created project and choose Properties
Change Configuration to Debug.
Configuration Properties > C/C++ > General > Additional Include Directories: Add C:\gtest\include
Configuration Properties > C/C++ > Code Generation > Runtime Library: If your code links to a runtime DLL, choose Multi-threaded Debug DLL (/MDd). If not, choose Multi-threaded Debug (/MTd).
Configuration Properties > Linker > General > Additional Library Directories: Add C:\gtest\msvc\gtest\Debug or C:\gtest\msvc\gtest-md\Debug, depending on the location of gtestd.lib
Configuration Properties > Linker > Input > Additional Dependencies: Add gtestd.lib
Verifying Everything Works

Open the cpp in your Test Project containing the main() function.
Paste the following code:
#include "stdafx.h"  
#include <iostream>

#include "gtest/gtest.h"

TEST(sample_test_case, sample_test)
{
    EXPECT_EQ(1, 1);
}

int main(int argc, char** argv) 
{ 
    testing::InitGoogleTest(&argc, argv); 
    RUN_ALL_TESTS(); 
    std::getchar(); // keep console window open until Return keystroke
}
