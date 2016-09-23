#include <TCPSocketDataLink.hpp>
#include <IDataStreamReceiver.hpp>

using namespace UnityRobot;
using namespace Networking;

#include <iostream>
#include <string>

/*
	Just a temporary class for testing connection with unity
*/
class ReceiverSample : public IDataStreamReceiver
{
public:

	virtual ~ReceiverSample() { };

	void IncomingData(const std::vector<char>& data)
	{
		std::cout << data.data() << std::endl;
	};
};

int main()
{
	TCPSocketDataLink link("localhost", "1234", std::unique_ptr<IDataStreamReceiver>(new ReceiverSample));

	std::cout << "Connected: " << ((link.Connected()) ? "SUCCEEDED" : "FAILED") << std::endl;

	if (link.Connected())
	{
		std::vector<char> dataToSend;
		dataToSend.push_back('Y');
		dataToSend.push_back('O');
		dataToSend.push_back('U');
		dataToSend.push_back('!');
		dataToSend.push_back('\n');

		link.SendData(dataToSend);
	}

	std::cin.ignore(1);

	return 0;
}