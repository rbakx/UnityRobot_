#include "ProtobufPresentation.h"
#include "RobotLogger.h"

void ProtobufPresentation::IncomingData(const std::vector<char>& data, Networking::IDataLink* dlink)
{
	receivedData.clear();
	receivedData.append(data.data(), data.size());

	lock.unlock();
}

void ProtobufPresentation::IncomingMessage(const Communication::Message& newMessage, Networking::IDataLink* dlink)
{
	//TODO
}

std::vector<char> ProtobufPresentation::MessageToBinaryData(const Communication::Message& message) const noexcept
{
	int size = message.ByteSize();
	auto msgStr = message.SerializeAsString();

	std::vector<char> result;
	result.push_back(size & 0xFF);
	result.push_back((size & 0xFF00) >> 8);
	result.push_back((size & 0xFF0000) >> 16);
	result.push_back((size & 0xFF000000) >> 24);
	std::copy(msgStr.begin(), msgStr.end(), std::back_inserter(result));

	return result;
}

Communication::Message ProtobufPresentation::BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) const
{
	int size = data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
	countedProcessedBytes = size;

	std::vector<char> msgData(data.begin() + 4, data.end());
	Communication::Message result;
	if (!result.ParseFromArray(msgData.data(), static_cast<int>(msgData.size())))
		LogError("Could not convert from binary data to message");

	return result;
}
