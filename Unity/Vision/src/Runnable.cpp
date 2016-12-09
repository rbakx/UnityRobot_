//
// Created by rutger on 9-12-16.
//

#include "Runnable.hpp"

Runnable::Runnable()
{}

void Runnable::Start()
{
	_running = true;

	_t = std::thread([=]() {
		while(_running)
		{
			this->run();
		}
	});
}

void Runnable::Stop()
{
	_running = false;
	_t.join();
}