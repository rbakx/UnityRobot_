#pragma once
#include <mutex>

#include "TCPSocketDataLink.hpp"
#include "IPresentationProtocol.hpp"

class ProtobufPresentation : public Networking::IPresentationProtocol
{
public:
	std::timed_mutex lock;
	std::string receivedData;

	void IncomingData(const std::vector<char>& data, Networking::IDataLink* dlink) override;
	void IncomingMessage(const Communication::Message& newMessage, Networking::IDataLink* dlink) override;
	std::vector<char> MessageToBinaryData(const Communication::Message& message) const noexcept override;
	Communication::Message BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) const override;
};
