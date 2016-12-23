#pragma once
#include "IMessageSender.hpp"

namespace Communication
{
	class Message;
}

namespace Networking
{
	class IPresentationProtocol;
	class IDataLink;
}

namespace UnityRobot
{

class Communicator : Networking::IMessageSender
{
public:
	Communicator(Networking::IPresentationProtocol& prot, Networking::IDataLink& data);
	bool sendCommand(const Communication::Message&) const override;

	Networking::IPresentationProtocol& getPresentationProtocol() const; //may have to change to non-const
	Networking::IDataLink& getDatalink() const; //may have to change to non-const
private:
	Networking::IPresentationProtocol& m_protocol;
	Networking::IDataLink& m_datalink;
};

}
