#pragma once

#include <vector>

#include <vector>
#include <stdint.h>

#include "message.pb.h"

using namespace Communication;

namespace Networking
{
    class IPresentationProtocolSerializer
    {
		public:
		
			virtual ~IPresentationProtocolSerializer(){};
			
			virtual std::vector<char> MessageToBinaryData(const Communication::Message& message) const noexcept = 0;
			virtual Communication::Message BinaryDataToMessage(const std::vector<char>& data, int32_t& countedProcessedBytes) const = 0;
    };
}