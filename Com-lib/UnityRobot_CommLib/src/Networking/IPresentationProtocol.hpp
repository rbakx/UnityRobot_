#pragma once

#include <vector>
#include <stdint.h>

#include <Message.h>

namespace Networking {
	
class IPresentationProtocol
{
	public:
	
		virtual ~IPresentationProtocol(){ };
		
		virtual std::vector<char> MessageToBinaryData(const Message& messsage) const noexcept = 0;
		virtual Message* BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) = 0;
};
	
}
