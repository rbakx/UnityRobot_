//
// Created by rutger on 9-12-16.
//
#pragma once

#include <thread>
#include <atomic>

class Runnable
{
	private:
		std::atomic<bool> _running;
		std::thread _t;

		virtual void run() = 0;

	public:
		Runnable();
		virtual ~Runnable();
		void Start();
		void Stop();
};