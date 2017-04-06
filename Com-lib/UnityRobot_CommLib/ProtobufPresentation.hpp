#pragma once

#include "TCPSocketDataLink.hpp"
#include "IPresentationProtocol.hpp"

class ProtobufPresentation : public Networking::IPresentationProtocol
{
	public:
	
		ProtobufPresentation();
		
		void SetReceiver(Networking::IMessageReceiver* receiver);
		void IncomingData(const std::vector<char>& data, Networking::IDataLink* datalink);
		
		std::vector<char> MessageToBinaryData(const Communication::Message& message) const noexcept;
		Communication::Message BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) const;
	
	private:
		
		Networking::IMessageReceiver* _messageReceiver;
		
		std::vector<char> _incomingData;
};
