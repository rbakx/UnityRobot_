#pragma once

#include "IDataStreamReceiver.hpp"
#include "IMessageReceiver.hpp"

#include "IPresentationProtocolSerializer.hpp"

namespace Networking
{
	class IPresentationProtocol : public IPresentationProtocolSerializer, public IDataStreamReceiver
	{
		public:

			virtual ~IPresentationProtocol(){ };

			virtual void SetReceiver(IMessageReceiver* receiver) = 0;
			virtual void IncomingData(const std::vector<char>& data, Networking::IDataLink* datalink) = 0;
			
			virtual std::vector<char> MessageToBinaryData(const Communication::Message& message) const noexcept = 0;
			virtual Communication::Message BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) const = 0;
	};
}
