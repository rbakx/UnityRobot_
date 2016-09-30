#include <TCPSocketDataLink.hpp>
#include <IDataStreamReceiver.hpp>

using namespace UnityRobot;
using namespace Networking;

#include <iostream>
#include <string>
#include <mutex>

/*
	Just a temporary class for testing connection with unity
*/
class ReceiverSample : public IDataStreamReceiver
{
public:

	std::timed_mutex lock;
	std::string receivedData;

	virtual ~ReceiverSample() { };

	void IncomingData(const std::vector<char>& data)
	{
		std::cout << "incoming!" << std::endl;
		
		receivedData.clear();
		receivedData.append(data.data(), data.size());
		lock.unlock();
	};
};

int main()
{
	ReceiverSample* _receiver = new ReceiverSample();

	TCPSocketDataLink link("145.93.45.166", "1234", std::unique_ptr<IDataStreamReceiver>(_receiver));

	link.Connect();

	std::cout << "Connected: " << ((link.Connected()) ? "SUCCEEDED" : "FAILED") << std::endl;

	if (link.Connected())
	{
		char binaryData[25];
		FillMemory(binaryData, 24, 0);

		memcpy(binaryData + 8, "Test message data", 17);

		binaryData[0] = 21;
		binaryData[4] = 8;
		binaryData[5] = 1;
		binaryData[6] = 18;
		binaryData[7] = 17;

		std::string binaryStr;
		binaryStr.append(binaryData, 25);

		std::string expectedResult[] =
		{
			binaryStr,
			//"longer testingString!",
			//"test",
			//"EVEN MOAR TEXT",
			//"EVEN BINARY \0 DATA!",
		};

		for (int i = 0; i < (sizeof(expectedResult) / sizeof(*expectedResult)); i++)
		{
			std::cout.write(expectedResult[i].c_str(), 25);
			std::cout << std::endl;
			link.SendData(expectedResult[i]);

			_receiver->lock.lock();

			if (_receiver->lock.try_lock_for(std::chrono::milliseconds(100)))
			{
				if (expectedResult[i] == _receiver->receivedData)
				{
					std::cout << "Match" << std::endl;
				}
			}
			else
			{
				//std::cout << "FAILURE TO READ" << std::endl;
			}

			_receiver->lock.unlock();
		}
	}

	std::cin.ignore(1);

	return 0;
}