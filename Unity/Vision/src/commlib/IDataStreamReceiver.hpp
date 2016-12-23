#pragma once

#include <vector>

namespace Networking {
class IDataLink;
class IDataStreamReceiver
{
	public:

		virtual ~IDataStreamReceiver() { };

		virtual void IncomingData(const std::vector<char>& data, IDataLink* dlink) = 0;
};

}
