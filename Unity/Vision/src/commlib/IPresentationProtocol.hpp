#pragma once

#include <vector>
#include <stdint.h>

#include "message.pb.h"
#include "IDataStreamReceiver.hpp"

namespace Networking {

class IPresentationProtocol : public IDataStreamReceiver
{
	public:

		virtual ~IPresentationProtocol(){ };

		virtual std::vector<char> MessageToBinaryData(const Communication::Message& messsage) const noexcept = 0;
		virtual Communication::Message BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) const = 0;
		virtual void IncomingMessage(const Communication::Message& newMessage, IDataLink* dlink) = 0;
};

}
