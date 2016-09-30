using Communication;
using System;

namespace Networking
{
	public interface IPresentationProtocol : IPresentationProtocolSerializer, IDataStreamReceiver
	{
		void SetReceiver(IMessageReceiver receiver);
		byte[] MessageToBinaryData(Message message);
		Message BinaryDataToMessage(byte[] data, out Int32 countedProcessedBytes);
		void IncomingData(byte[] data);
	}
}
