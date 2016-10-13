#pragma once

#include <Message.h>

namespace Networking {

class IMessageReceiver
{
	public:

		virtual ~IMessageReceiver(){ };

		virtual void IncomingMessage(const Message& newMessage, IDataLink* dlink) = 0;
};

}
