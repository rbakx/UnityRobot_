#pragma once

#include <vector>

namespace Networking {
	
class IDataStreamReceiver
{
	public:
	
		virtual ~IDataStreamReceiver() { };
		
		virtual void IncomingData(const std::vector<char>& data) = 0;
};
	
}
