#pragma once

#include <Message.h>

namespace Networking {
	
class IMessageSender
{
	public:
	
		virtual ~IMessageSender(){ };
		
		virtual void IncomingMessage(const Message& newMessage) = 0;
};
	
}
