//
// Created by rutger on 9-12-16.
//
#include <iostream>
#include "Runnable.hpp"

Runnable::Runnable()
	: _running(false)
{}

Runnable::~Runnable()
{
	if(_running)
	{
		
	}
}

void Runnable::Start()
{

	_t = std::thread([this](std::atomic<bool>& running) {
		running = true;

		while(running)
		{
			this->run();
		}

	}, std::ref(_running));
}

//TODO: Rename Stop -> StopRunnable
void Runnable::Stop()
{
	_running = false;
	_t.join();
}