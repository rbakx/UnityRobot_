#include <TCPSocketDataLink.hpp>

#include <asio/impl/connect.hpp>
#include <asio/impl/write.hpp>
#include <asio/io_service.hpp>
#include <stdexcept>

namespace UnityRobot {

asio::io_service TCPSocketDataLink::_Service;

TCPSocketDataLink::TCPSocketDataLink(std::string address, std::string port, std::unique_ptr<Networking::IDataStreamReceiver>& receiver)
	: m_AdrressStr(std::move(address)), m_SocketStr(std::move(port)), m_Socket(TCPSocketDataLink::_Service),
	m_Resolver(TCPSocketDataLink::_Service), m_TCPReaderThread(std::thread()), m_receiver(receiver.release())
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

TCPSocketDataLink::~TCPSocketDataLink()
{
	m_Socket.close();

	if (m_TCPReaderThread.joinable())
	{ m_TCPReaderThread.join(); }
}

void TCPSocketDataLink::Connect()
{
	try
	{
		asio::connect(m_Socket, m_Resolver.resolve({ m_AdrressStr, m_SocketStr }));

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
		//e.what();
		//LOG : CONNECTION FAILED!

		m_Socket.close();
	}
}

void TCPSocketDataLink::StartReading()
{
	asio::error_code ec;

	std::vector<char> socketBuffer;
	socketBuffer.reserve(128);

	while (ec != asio::error::eof)
	{
		size_t buffersize = m_Socket.read_some(asio::buffer(socketBuffer, 128), ec);

		if (buffersize > 0)
		{
			m_receiver->IncomingData(socketBuffer);

			//std::cout << "received" << std::endl;

			for (unsigned int i = 0; i < buffersize; ++i)
			{
				//std::cout << socketBuffer.at(i);
			}

			//std::cout << "" << std::endl;

			socketBuffer.clear();
		}
	}

	m_Socket.close();
}

void TCPSocketDataLink::ReadCallback(const asio::error_code& error, size_t bytes_transferred)
{
	if(error)
	{
		
		//logstuff
		return;
	}
}

bool TCPSocketDataLink::SendData(const std::vector<char>& data) noexcept
{
	bool result = ( asio::write(m_Socket, asio::buffer(data.data(), data.size())) == data.size() );

	if (!result)
	{
		//LOG THIS SHIT
	}

	return result;
}

bool TCPSocketDataLink::Connected() const noexcept
{
	return m_Socket.is_open();
}

}