#pragma once

#include <asio.hpp>

class Ev3Wifi 
{
private:
	asio::io_service m_ioService;

public:
	Ev3Wifi();
	~Ev3Wifi();

	bool connect();
	void disconnect();	
};