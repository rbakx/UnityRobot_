#pragma once

#include <vector>

namespace Networking {
	
class IDataLink
{
	public:
	
		virtual ~IDataLink() { };
		
		virtual bool SendData(const std::vector<char>& data) noexcept = 0;
		virtual bool Connected() const noexcept = 0;
};
	
}
