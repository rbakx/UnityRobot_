#pragma once

#include <vector>
#include <string>

namespace Networking {
	
class IDataLink
{
	public:
	
		virtual ~IDataLink() { };
		
		virtual void Connect() = 0;
		virtual bool SendData(const std::string& data) noexcept = 0;
		virtual bool SendData(const std::vector<char>& data) noexcept = 0;
		virtual bool Connected() const noexcept = 0;
};
	
}
