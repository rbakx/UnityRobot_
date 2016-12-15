#include "communicator.h"
#include "IPresentationProtocol.hpp"
#include "IDataLink.hpp"
#include "RobotLogger.h"
namespace UnityRobot
{

Communicator::Communicator(Networking::IPresentationProtocol& prot, Networking::IDataLink& data)
	: m_protocol(prot), m_datalink(data)
{
	if (!m_datalink.Connected())
	{
		LogCritError("Datalink provided to communicator is not connected");
		throw std::invalid_argument("Datalink is not connected");
	}
}

bool Communicator::sendCommand(const Communication::Message& m) const
{
	return m_datalink.Connected() ? m_datalink.SendData(m_protocol.MessageToBinaryData(m)) : false;
}

Networking::IDataLink& Communicator::getDatalink() const
{
	return m_datalink;
}

Networking::IPresentationProtocol& Communicator::getPresentationProtocol() const
{
	return m_protocol;
}

}