using Communication;
using Networking;
using NUnit.Framework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

[TestFixture]
public class TEST_ProtoBufPresentation
{
	[Test]
    public void BinaryDataToMessage()
    {
        // Setup
        ProtoBufPresentation pbPres = new ProtoBufPresentation();
		IMessageReceiver receiver = NSubstitute.Substitute.For<IMessageReceiver>();
		pbPres.SetReceiver(receiver);

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
        ProtoBufPresentation pbPres = new ProtoBufPresentation();
		IMessageReceiver receiver = NSubstitute.Substitute.For<IMessageReceiver>();
		pbPres.SetReceiver(receiver);

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

    [Test]
    public void IncomingDataToReceiver()
    {
        int messageTarget = 1;
        string messageData = "Test message data";

        Message expectedMessage = new Message { target = messageTarget, data = messageData };
        Message sourceMessage = new Message { target = messageTarget, data = messageData };

        ProtoBufPresentation pbPres = new ProtoBufPresentation();
		DummyMessageReceiver receiver = new DummyMessageReceiver(expectedMessage);
		pbPres.SetReceiver(receiver);

        byte[] binaryMessage = pbPres.MessageToBinaryData(sourceMessage);

        pbPres.IncomingData(binaryMessage);
        pbPres.IncomingData(binaryMessage);

        byte[] firstHalf = new byte[6];
        byte[] secondHalf = new byte[binaryMessage.Length - 6];

        Array.Copy(binaryMessage, 0, firstHalf, 0, 6);
        Array.Copy(binaryMessage, 6, secondHalf, 0, binaryMessage.Length - 6);

        pbPres.IncomingData(firstHalf);
        pbPres.IncomingData(secondHalf);
    }
}

public class DummyMessageReceiver : IMessageReceiver
{
    private Message _expectedMessage;

    public DummyMessageReceiver(Message expectedMessage)
    {
        _expectedMessage = expectedMessage;
    }

    public void IncomingMessage(Message newMessage)
    {
        Assert.NotNull(newMessage);
        Assert.AreEqual(newMessage.target, _expectedMessage.target);
        Assert.AreEqual(newMessage.data, _expectedMessage.data);
    }
}