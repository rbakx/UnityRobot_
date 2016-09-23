#include "RobotSocket.h"
#include <asio/impl/connect.hpp>
#include <asio/impl/write.hpp>
#include <asio/io_service.hpp>

namespace UnityRobot {
asio::io_service RobotSocket::_Service;

RobotSocket::RobotSocket(std::string address, std::string socket)
	: m_AdrressStr(address), m_SocketStr(socket), m_Socket(RobotSocket::_Service), m_Resolver(RobotSocket::_Service)
{

}

void RobotSocket::Connect()
{
	asio::connect(m_Socket, m_Resolver.resolve({m_AdrressStr, m_SocketStr}));
}

size_t RobotSocket::Write(std::vector<char> data)
{
	return asio::write(m_Socket, asio::buffer(data.data(), data.size()));
}

void RobotSocket::Read()
{

}

void RobotSocket::ReadCallback(const asio::error_code& error, size_t bytes_transferred)
{
	if(error)
	{
		
		//logstuff
		return;
	}


}


}
