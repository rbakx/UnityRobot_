using NUnit.Framework;
using System;
using Networking;
using Communication;
using System.IO;
using ProtoBuf;
using System.Collections.Generic;

[TestFixture]
public class TEST_ProtoBufPresentation {

    [Test]
    [ExpectedException (typeof(ArgumentNullException))]
    public void ConstructorWithNullParam()
    {
        ProtoBufPresentation pbPresentation = new ProtoBufPresentation(null);
    }

    [Test]
    public void BinaryDataToMessage()
    {
        // Setup
        IMessageReceiver receiver = NSubstitute.Substitute.For<IMessageReceiver>();
        ProtoBufPresentation pbPres = new ProtoBufPresentation(receiver);

        int messageTarget = 1;
        string messageData = "Test message data";

        Message sourceMessage = new Message { target = messageTarget, data = messageData };

        MemoryStream stream = new MemoryStream();
        Serializer.Serialize(stream, sourceMessage);

        List<byte> binaryData = new List<byte>();
        byte[] data = stream.ToArray();

        binaryData.AddRange(BitConverter.GetBytes((Int32)data.Length));
        binaryData.AddRange(data);

        // Deserializing
        //Message result = 
    }

}
