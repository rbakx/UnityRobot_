//
// Created by rutger on 9-12-16.
//
#include <iostream>
#include "Runnable.hpp"

Runnable::Runnable() noexcept
	: _running(false)
{}

Runnable::~Runnable()
{
	if(_running)
	{
		std::cerr << "[Runnable] Stop() was not called before Runnable's destructor" << std::endl;
		Stop();
	}
}

void Runnable::Start() noexcept
{
	/*
		TODO: mutex
		Note: possible concurrency issue here!
		If another thread calls start at the same time, _t will be overwritten twice.
		The only way to deal with this issue is to use a mutex or another rapid locking mechanism.
		If Start is not called often or from other threads, ignore the issue.
	*/
	if(!_running)
	{
		_running = true;
		/*
			When supplying 'this' as argument, all private members stay available
			Be aware of concurrency issues; This will not happen for single byte variables such as bool, uint8 and char.
		*/
		_t = std::thread([this]()
		{
			while(this->_running)
			{
				this->run();
			}

		});
	}
}

//TODO: Rename Stop -> StopRunnable
void Runnable::Stop() noexcept
{
	_running = false;

	if(_t.joinable())
	{
		_t.join();
	}
}

bool Runnable::IsRunning() noexcept
{
	return (_running || _t.joinable());
}