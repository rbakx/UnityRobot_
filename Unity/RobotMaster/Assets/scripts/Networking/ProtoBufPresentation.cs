using UnityEngine;
using System.Collections;
using System;

using Communication;
using ProtoBuf;
using System.IO;
using System.Collections.Generic;

namespace Networking
{
    public class ProtoBufPresentation : IPresentationProtocol
    {
        private IMessageReceiver _messageReceiver;
        private List<byte> _incomingData;

        public ProtoBufPresentation(IMessageReceiver receiver)
        {
            if (receiver == null)
            {
                throw new ArgumentNullException("receiver");
            }

            _messageReceiver = receiver;
            _incomingData = new List<byte>();
        }

        public Message BinaryDataToMessage(byte[] data)
        {
            Message result = null;

            if (data.Length > sizeof(Int32))
            {
                Int32 messageSize = BitConverter.ToInt32(data, 0);

                Int32 dataLength = data.Length - sizeof(Int32);

                if (dataLength >= messageSize)
                {
                    MemoryStream dataStream = new MemoryStream(data, sizeof(Int32), dataLength);
                    result = Serializer.Deserialize<Message>(dataStream);

                    // Remove the deserialized data from the incoming data
                    _incomingData.RemoveRange(0, sizeof(Int32) + dataLength);
                }
            }

            return result;
        }

        public void IncomingData(byte[] data)
        {
            _incomingData.AddRange(data);

            Message message = BinaryDataToMessage(_incomingData.ToArray());

            if (message != null)
            {
                _messageReceiver.IncomingMessage(message);
            }
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
