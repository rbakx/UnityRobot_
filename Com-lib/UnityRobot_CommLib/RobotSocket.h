#pragma once
#define ASIO_STANDALONE 
#define ASIO_HAS_STD_CHRONO
#define ASIO_HAS_STD_ADDRESSOF
#define ASIO_HAS_STD_ARRAY
#define ASIO_HAS_CSTDINT
#define ASIO_HAS_STD_SHARED_PTR
#define ASIO_HAS_STD_TYPE_TRAITS
#include <asio.hpp>
#include <asio/io_service.hpp>
#include <asio/ip/tcp.hpp>

namespace UnityRobot
{
class RobotSocket
{
public:
	RobotSocket(std::string address, std::string socket);

	void Connect();

	size_t Write(std::vector<char> data);
	void Read();

	void ReadCallback(const asio::error_code& error, std::size_t bytes_transferred);
private:
	std::string m_AdrressStr;
	std::string m_SocketStr;

	asio::ip::tcp::socket m_Socket;
	asio::ip::tcp::resolver m_Resolver;
private:
	static asio::io_service _Service;
};

}
