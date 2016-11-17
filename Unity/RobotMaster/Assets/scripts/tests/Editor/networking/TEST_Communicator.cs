using Communication;
using Networking;
using NUnit.Framework;

namespace NetworkingTests
{

    [TestFixture]
    public class TEST_Communicator
    {

        [Test]
        public void CommunicatorSendAndRecieve()
        {
            DummyReceiver receiver = new DummyReceiver();
            ProtoBufPresentation pp = new ProtoBufPresentation();

            pp.SetReceiver(receiver);

            DummyDataLink datalink = new DummyDataLink(pp);


            Communicator com = new Communicator(datalink, pp);

            Message sendMessage = new Message { id = 2, stringData = "Message data..." };

            Assert.True(com.SendCommand(sendMessage));

            Assert.NotNull(receiver.incomingMessage);

            Assert.AreEqual(receiver.incomingMessage.id, sendMessage.id);
            Assert.AreEqual(receiver.incomingMessage.stringData, sendMessage.stringData);
        }
    }

    class DummyDataLink : IDataLink
    {
        IDataStreamReceiver receiver;

        public DummyDataLink(IDataStreamReceiver receiver)
        {
            this.receiver = receiver;
        }

        public bool SendData(byte[] data)
        {
            receiver.IncomingData(data, this);
            return true;
        }

        public bool Connected()
        {
            return true;
        }

        public void SetReceiver(IDataStreamReceiver receiver)
        {
            this.receiver = receiver;
        }

        public void Dispose()
        {

        }
    }

    class DummyReceiver : IMessageReceiver
    {
        public Message incomingMessage;

        public void IncomingMessage(Message incomingMessage, IDataLink datalink)
        {
            this.incomingMessage = incomingMessage;
        }
    }
}