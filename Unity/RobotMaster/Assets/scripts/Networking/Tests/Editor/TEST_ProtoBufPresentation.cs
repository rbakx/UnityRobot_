using NUnit.Framework;
using System;
using Networking;
using Communication;
using System.IO;
using ProtoBuf;
using System.Collections.Generic;

[TestFixture]
public class TEST_ProtoBufPresentation
{

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
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
        Message result = pbPres.BinaryDataToMessage(binaryData.ToArray());

        // Checking
        Assert.NotNull(result);
        Assert.AreEqual(result.target, messageTarget);
        Assert.AreEqual(result.data, messageData);
    }

    [Test]
    public void MessageToBinaryData()
    {
        // Setup
        IMessageReceiver receiver = NSubstitute.Substitute.For<IMessageReceiver>();
        ProtoBufPresentation pbPres = new ProtoBufPresentation(receiver);

        int messageTarget = 1;
        string messageData = "Test message data";

        Message sourceMessage = new Message { target = messageTarget, data = messageData };

        MemoryStream stream = new MemoryStream();
        Serializer.Serialize<Message>(stream, sourceMessage);

        byte[] binaryData = stream.ToArray();

        // Serializing
        byte[] resultData = pbPres.MessageToBinaryData(sourceMessage);

        // Checking
        // Size
        Assert.AreEqual(binaryData.Length, BitConverter.ToInt32(resultData, 0));
        // Data
        for (int i = 0; i < binaryData.Length; i++)
        {
            Assert.AreEqual(binaryData[i], resultData[i + sizeof(Int32)]);
        }

    }

}
