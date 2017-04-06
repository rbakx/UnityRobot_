#include "TCPSocketDataLink.hpp"

#include <stdexcept>
#include "RobotLogger.h"

namespace UnityRobot {

boost::asio::io_service TCPSocketDataLink::_Service;

TCPSocketDataLink::TCPSocketDataLink(std::string address, std::string port, std::unique_ptr<Networking::IDataStreamReceiver>& receiver)
	: m_TCPReaderThread(std::thread()), m_AdrressStr(std::move(address)), m_SocketStr(std::move(port)),
	m_Socket(TCPSocketDataLink::_Service), m_Resolver(TCPSocketDataLink::_Service), m_receiver(receiver.release())
{
	if (m_AdrressStr.length() < 3)
	{
		throw std::invalid_argument("[TCPSocketDataLink] String addres must be a hostname name.ext or IP address or 0.0.0.0");
	}

	int portValue =	stoi(m_SocketStr);

	if (portValue <= 0 || portValue > 65655)
	{
		throw std::out_of_range("[TCPSocketDataLink] port should be within range 1 - 65655");
	}

	if (m_receiver == nullptr)
	{
		throw std::invalid_argument("[TCPSocketDataLink] IDataStreamReceiver receiver may not own null");
	}
}

TCPSocketDataLink::TCPSocketDataLink(std::string address, std::string port, std::unique_ptr<Networking::IDataStreamReceiver>&& receiver)
	: m_TCPReaderThread(std::thread()), m_AdrressStr(std::move(address)), m_SocketStr(std::move(port)),
	m_Socket(TCPSocketDataLink::_Service), m_Resolver(TCPSocketDataLink::_Service), m_receiver(receiver.release())
{
	if (m_AdrressStr.length() < 3)
	{
		throw std::invalid_argument("[TCPSocketDataLink] String addres must be a hostname name.ext or IP address or 0.0.0.0");
	}

	int portValue = stoi(m_SocketStr);

	if (portValue <= 0 || portValue > 65655)
	{
		throw std::out_of_range("[TCPSocketDataLink] port should be within range 1 - 65655");
	}

	if (m_receiver == nullptr)
	{
		throw std::invalid_argument("[TCPSocketDataLink] IDataStreamReceiver receiver may not own null");
	}
}

TCPSocketDataLink::~TCPSocketDataLink()
{
	LogInfo(std::string("Closing connection on " + m_AdrressStr + ":" + m_SocketStr));
	m_Socket.close();

	if (m_TCPReaderThread.joinable())
	{ m_TCPReaderThread.join(); }
}

void TCPSocketDataLink::Connect()
{
	try
	{
		boost::asio::connect(m_Socket, m_Resolver.resolve({ m_AdrressStr, m_SocketStr }));

		if (Connected())
		{
			m_TCPReaderThread = std::thread([this]()
			{
				StartReading();
			});
		}
	}
	catch (std::exception& e)
	{
		LogError(e.what());
		m_Socket.close();
	}
}

void TCPSocketDataLink::StartReading()
{
	boost::system::error_code ec;

	std::vector<char> socketBuffer(128);
	while (ec != boost::asio::error::eof && m_Socket.is_open())
	{
		size_t buffersize = m_Socket.read_some(boost::asio::buffer(socketBuffer, 128), ec);

		if (buffersize > 0)
		{
			std::vector<char> incomingDataString;
			std::copy(socketBuffer.begin(), socketBuffer.begin() + buffersize, std::back_inserter(incomingDataString));
			m_receiver->IncomingData(incomingDataString, this);
		}
	}

	m_Socket.close();
}

void TCPSocketDataLink::ReadCallback(const boost::system::error_code& error, size_t bytes_transferred)
{
	if(error)
	{

		//logstuff
		return;
	}
}

bool TCPSocketDataLink::SendData(const std::string& data) noexcept
{
	std::vector<char> charVectorArray;
	charVectorArray.reserve(data.length());

	std::copy(data.begin(), data.end(), std::back_inserter(charVectorArray));

	return SendData(charVectorArray);
}

bool TCPSocketDataLink::SendData(const std::vector<char>& data) noexcept
{
	bool result = ( boost::asio::write(m_Socket, boost::asio::buffer(data.data(), data.size())) == data.size() );

	if (!result)
	{
		LogError("Failed to send data through TCPSocketDatalink");
	}

	return result;
}

bool TCPSocketDataLink::Connected() const noexcept
{
	return m_Socket.is_open();
}

Networking::IDataStreamReceiver* TCPSocketDataLink::getReceiver() const
{
	return m_receiver.get();
}
}
