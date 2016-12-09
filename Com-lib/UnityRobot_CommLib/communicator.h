#pragma once
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

class Communicator
{
public:
	Communicator(Networking::IPresentationProtocol& prot, Networking::IDataLink& data);
	bool sendCommand(const Communication::Message&) const;
private:
	Networking::IPresentationProtocol& m_protocol;
	Networking::IDataLink& m_datalink;
};

}
