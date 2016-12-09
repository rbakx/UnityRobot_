//
// Created by rutger on 9-12-16.
//
#pragma once

#include <thread>

class Runnable
{
private:
	bool _running;
	std::thread _t;
	virtual void run() = 0;

public:
	Runnable();
	void start();
	void stop();
};