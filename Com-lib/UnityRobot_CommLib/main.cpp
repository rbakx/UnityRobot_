#include <TCPSocketDataLink.hpp>
#include <IDataStreamReceiver.hpp>
#include <IPresentationProtocol.hpp>
using namespace UnityRobot;
using namespace Networking;

#include <iostream>
#include <string>
#include <mutex>
#include "RobotLogger.h"
#include <message.pb.h>
/*
	Just a temporary class for testing connection with unity
*/
class ReceiverSample : public IDataStreamReceiver
{
public:

	std::timed_mutex lock;
	std::string receivedData;

	virtual ~ReceiverSample() { };

	void IncomingData(const std::vector<char>& data, IDataLink* ) override
	{
		std::cout << "incoming!" << std::endl;
		
		receivedData.clear();
		receivedData.append(data.data(), data.size());
		lock.unlock();
	};
};

class PresentationSample : public IPresentationProtocol
{
public:
	std::timed_mutex lock;
	std::string receivedData;

	//virtual ~ReceiverSample() { };
	//
	//void IncomingData(const std::vector<char>& data, IDataLink*) override
	//{
	//	std::cout << "incoming!" << std::endl;
	//
	//	receivedData.clear();
	//	receivedData.append(data.data(), data.size());
	//	lock.unlock();
	//};
	void IncomingData(const std::vector<char>& data, IDataLink* dlink) override
	{
		
	}

	void IncomingMessage(const Communication::Message& newMessage, IDataLink* dlink) override
	{
		
	}

	std::vector<char> MessageToBinaryData(const Communication::Message& message) const noexcept override
	{
		int size = message.ByteSize();
		auto msgStr = message.SerializeAsString();

		std::vector<char> result;
		result.push_back(size & 0xFF);
		result.push_back((size & 0xFF00) >> 8);
		result.push_back((size & 0xFF0000) >> 16);
		result.push_back((size & 0xFF000000) >> 24);
		std::copy(msgStr.begin(), msgStr.end(), std::back_inserter(result));

		std::cout << result.data() << std::endl;

		return result;
	}

	Communication::Message BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) const override
	{
		int size = data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
		countedProcessedBytes = size;

		std::vector<char> msgData(data.begin() + 4, data.end());
		Communication::Message result;
		//if (!result.ParseFromArray(msgData.data(), static_cast<int>(msgData.size())))
			//LogError("Could not convert from binary data to message");

		return result;
	}
};

int main(int argc, char** argv)
{
	//RobotLogger logger;
	//logger.init();
	GOOGLE_PROTOBUF_VERIFY_VERSION;

	std::string address = argc > 2 ?  argv[1] : "145.93.44.64";
	std::string port = argc > 2 ? argv[2] : "1234";

	//ReceiverSample* _receiver = new ReceiverSample();
	PresentationSample* _receiver = new PresentationSample();
	TCPSocketDataLink link(address, port, std::unique_ptr<IDataStreamReceiver>(_receiver));

	link.Connect();

	std::cout << "Connected: " << ((link.Connected()) ? "SUCCEEDED" : "FAILED") << std::endl;

	if (link.Connected())
	{
		Communication::Message toSend;
		toSend.set_messagetype(Communication::MessageType::Identification);
		toSend.set_messagetarget(Communication::MessageTarget::Unity);
		toSend.set_id(13);
	
		auto r = toSend.mutable_stringdata();
		r->append("stupid_robot");

		auto upd = toSend.mutable_shapeupdate();
		auto ch = upd->add_changedshapes();
		ch->set_id(3);
		auto vertex = ch->add_vertices();
		vertex->set_x(5.0);
		vertex->set_y(5.5);
		vertex->set_z(1.0);

		auto vertex2 = ch->add_vertices();
		vertex2->set_x(1.0);
		vertex2->set_y(1.5);
		vertex2->set_z(1.5);


		auto upd2 = toSend.mutable_shapeupdate();
		auto ch2 = upd->add_changedshapes();
		ch2->set_id(5);
		auto vertex3 = ch2->add_vertices();
		vertex3->set_x(-5.0);
		vertex3->set_y(-5.5);
		vertex3->set_z(-1.0);

		std::cout << link.SendData(_receiver->MessageToBinaryData(toSend));

	}

	std::cin.ignore(1);

	return 0;
}