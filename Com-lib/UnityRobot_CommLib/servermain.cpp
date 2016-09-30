#define ASIO_STANDALONE 
#define ASIO_HAS_STD_CHRONO
#define ASIO_HAS_STD_ADDRESSOF
#define ASIO_HAS_STD_ARRAY
#define ASIO_HAS_CSTDINT
#define ASIO_HAS_STD_SHARED_PTR
#define ASIO_HAS_STD_TYPE_TRAITS

#include <asio.hpp>

//#include <functional>
#include <iostream>

#include <cstdlib>
//#include <iostream>
#include <memory>
//#include <utility>
//#include <boost/asio.hpp>
//using boost::asio::ip::tcp;

class session
	: public std::enable_shared_from_this<session>
{
public:
	session(asio::ip::tcp::socket socket)
		: socket_(std::move(socket))
	{
	}

	void start()
	{
		do_read();
	}

private:
	void do_read()
	{
		auto self(shared_from_this());
		socket_.async_read_some(asio::buffer(data_, max_length),
			[this, self](asio::error_code ec, std::size_t length)
		{
			if (!ec)
			{
				//data_[length - 1] = 'r';
				//data_[length] = 'e';
				//data_[length + 1] = 'c';
				std::string s(asio::buffer_cast<const char*>(asio::buffer(data_, length)), length);
				//std::cout << s << " was the message\n";
				std::cout.write(s.data(), length);
				std::cout << ", length: " << length << std::endl;

				do_write(length);
			}
		});
	}

	void do_write(std::size_t length)
	{
		auto self(shared_from_this());
		//asio::async_write(socket_, asio::buffer(data_, length),

		std::cout << "data: ";
		std::cout.write(data_, length);
		std::cout << std::endl;

		asio::async_write(socket_, asio::buffer(data_, length),
			[this, self](asio::error_code ec, std::size_t /*length*/)
		{

			if (!ec)
			{
				//std::string s(asio::buffer_cast<const char*>(asio::buffer(data_, length)), length);
				//std::cout << s << "was the message\n";
				//std::string s(asio::buffer_cast<const char*>(b.data()), b.size());
				//std::cout << asio::buffer(data_, length).begin();
				do_read();
			}
		});
	}

	asio::ip::tcp::socket socket_;
	enum { max_length = 1024 };
	char data_[max_length];
};

class server
{
public:
	server(asio::io_service& io_service, short port)
		: acceptor_(io_service, asio::ip::tcp::endpoint(asio::ip::tcp::v4(), port)),
		socket_(io_service)
	{
		do_accept();
	}

private:
	void do_accept()
	{
		acceptor_.async_accept(socket_,
			[this](asio::error_code ec)
		{
			std::cout << "Incoming client from " << socket_.remote_endpoint().address().to_string() << std::endl;

			if (!ec)
			{
				std::make_shared<session>(std::move(socket_))->start();
			}

			do_accept();
		});
	}

	asio::ip::tcp::acceptor acceptor_;
	asio::ip::tcp::socket socket_;
};

int main(int argc, char* argv[])
{
	std::cout << "Program started" << std::endl;

	try
	{
		if (argc != 2)
		{
			std::cerr << "Usage: async_tcp_echo_server <port>\n";
			return 1;
		}

		int portValue = std::atoi(argv[1]);

		std::cout << "Attempting to listen on port: " << portValue << std::endl;

		asio::io_service io_service;

		server s(io_service, portValue);

		io_service.run();
	}
	catch (std::exception& e)
	{
		std::cerr << "Exception: " << e.what() << "\n";
	}

	return 0;
}
//int main(int argc, char** argv)
//{
//	asio::io_service serv;
//	asio::steady_timer timer1{ serv, std::chrono::seconds{ 3 } };
//	timer1.async_wait([](const asio::error_code &ec)
//	{ std::cout << "3 sec\n"; });
//
//	asio::steady_timer timer2{ serv, std::chrono::seconds{ 4 } };
//	timer2.async_wait([](const asio::error_code &ec)
//	{ std::cout << "4 sec\n"; });
//
//	asio::steady_timer timer3{ serv, std::chrono::seconds{ 15 } };
//	timer3.async_wait([](const asio::error_code &ec)
//	{ std::cout << "15 sec\n"; });
//
//	//auto s = std::placeholders::_1;
//
//	serv.run();
//}