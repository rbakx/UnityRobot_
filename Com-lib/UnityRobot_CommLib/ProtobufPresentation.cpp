#include "ProtobufPresentation.hpp"
#include "RobotLogger.h"

using namespace Networking;

ProtobufPresentation::ProtobufPresentation() : _messageReceiver(nullptr)
{
	
}

void ProtobufPresentation::SetReceiver(IMessageReceiver* receiver)
{
	// Lock makes sure _messageReceiver doesn't go out of scope while it is being used during IncomingData call.
	if(receiver == nullptr)
	{
		throw std::invalid_argument("receiver");
	}	
			
	_messageReceiver = receiver;
}

void ProtobufPresentation::IncomingData(const std::vector<char>& data, Networking::IDataLink* datalink)
{
	_incomingData.insert(std::end(_incomingData), std::begin(data), std::end(data));

	int32_t processedBytes;
	
	do
	{
		Message message = BinaryDataToMessage(_incomingData, processedBytes);

		if(message.id() > 0) // message != nullptr)
		{
			if(_messageReceiver != nullptr)
			{
				_messageReceiver->IncomingMessage(message, datalink);
			}

			// Remove the deserialized data from the incoming data
			_incomingData.erase(_incomingData.begin(), _incomingData.begin() + processedBytes);
		}
	} while (processedBytes > 0);
}

std::vector<char> ProtobufPresentation::MessageToBinaryData(const Communication::Message& message) const noexcept
{	
	const int32_t size = message.ByteSize();
	const auto& msgStr = message.SerializeAsString();

	std::vector<char> result;
	
	result.reserve(sizeof(size) + msgStr.length());
	
	char* data_start = result.data();
	
	memcpy(data_start, &size, sizeof(size));
	memcpy(data_start + sizeof(size), msgStr.c_str(), msgStr.length());
	
	return result;
}

Communication::Message ProtobufPresentation::BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) const
{
	Communication::Message result;
	result.set_id(0);

	countedProcessedBytes = 0;
	
	if(data.size() > sizeof(int32_t))
	{
		const char* data_start = data.data();
	
		auto messageSize = *(const int32_t*)(data_start);
		
		auto dataLength = data.size() - sizeof(messageSize);
		
		if(dataLength >= messageSize)
		{
			if (result.ParseFromArray(data.data(), messageSize))
			{
				if(result.id() == 0) { result.set_id(1); };
				countedProcessedBytes = messageSize + sizeof(int32_t);
			}
			else
			{
				LogError("Could not convert from binary data to message");
			}
		}
	}

	return std::move(result);
}
