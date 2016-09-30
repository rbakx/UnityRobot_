using Communication;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Networking
{
	public class ProtoBufPresentation : IPresentationProtocol
	{
		private IMessageReceiver _messageReceiver;
		private List<byte> _incomingData;

		public ProtoBufPresentation()
		{
			_incomingData = new List<byte>();
		}

		public void SetReceiver(IMessageReceiver receiver)
		{
			if(receiver == null)
			{
				throw new ArgumentNullException("receiver");
			}

			_messageReceiver = receiver;
		}

		public Message BinaryDataToMessage(byte[] data)
		{
			Int32 processedBytes;
			return BinaryDataToMessage(data, out processedBytes);
		}

		public Message BinaryDataToMessage(byte[] data, out Int32 processedBytes)
		{
			Message result = null;
			processedBytes = 0;

			if(data.Length > sizeof(Int32))
			{
				Int32 messageSize = BitConverter.ToInt32(data, 0);

				Int32 dataLength = data.Length - sizeof(Int32);

				if(dataLength >= messageSize)
				{
					MemoryStream dataStream = new MemoryStream(data, sizeof(Int32), messageSize);
					result = Serializer.Deserialize<Message>(dataStream);

					processedBytes = messageSize + sizeof(Int32);
				}
			}

			return result;
		}

		public void IncomingData(byte[] data)
		{
			_incomingData.AddRange(data);

			Int32 processedBytes;
			do
			{
				Message message = BinaryDataToMessage(_incomingData.ToArray(), out processedBytes);

				if(message != null)
				{

					if(_messageReceiver != null)
					{
						_messageReceiver.IncomingMessage(message);
					}

					// Remove the deserialized data from the incoming data
					_incomingData.RemoveRange(0, processedBytes);
				}
			} while (processedBytes != 0);
		}

		public byte[] MessageToBinaryData(Message message)
		{
			List<byte> result = new List<byte>();

			MemoryStream messageStream = new MemoryStream();
			Serializer.Serialize<Message>(messageStream, message);

			byte[] messageData = messageStream.ToArray();

			// Insert size
			result.AddRange(BitConverter.GetBytes((Int32)messageData.Length));

			// Insert data
			result.AddRange(messageData);

			return result.ToArray();
		}
	}
}
