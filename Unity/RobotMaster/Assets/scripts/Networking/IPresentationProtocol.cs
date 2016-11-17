using Communication;
using System;

namespace Networking
{
	public interface IPresentationProtocol : IPresentationProtocolSerializer, IDataStreamReceiver
	{
		void SetReceiver(IMessageReceiver receiver);
		Message BinaryDataToMessage(byte[] data, out Int32 countedProcessedBytes);
	}
}
