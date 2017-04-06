#pragma once

#include <message.pb.h>
#include <IDataLink.hpp>

namespace Networking {

class IMessageReceiver
{
	public:

		virtual ~IMessageReceiver(){ };

		virtual void IncomingMessage(const Communication::Message& newMessage, Networking::IDataLink* dlink) = 0;
};

}
