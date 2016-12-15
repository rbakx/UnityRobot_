#pragma once

namespace Communication
{
	class Message;
}

namespace Networking {

class IMessageSender
{
	public:

		virtual ~IMessageSender(){ };

		virtual bool sendCommand(const Communication::Message& newMessage) const = 0;
};

}
