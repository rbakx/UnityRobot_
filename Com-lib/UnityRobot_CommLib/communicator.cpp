#include "communicator.h"
#include "IPresentationProtocol.hpp"
#include "IDataLink.hpp"
namespace UnityRobot
{

Communicator::Communicator(Networking::IPresentationProtocol& prot, Networking::IDataLink& data)
	: m_protocol(prot), m_datalink(data)
{
}

bool Communicator::sendCommand(const Communication::Message& m) const
{
	//???? Is this necessary? Who knows
	//m_datalink.Connect();
	//???? end
	return m_datalink.Connected() ? m_datalink.SendData(m_protocol.MessageToBinaryData(m)) : false;
}

}