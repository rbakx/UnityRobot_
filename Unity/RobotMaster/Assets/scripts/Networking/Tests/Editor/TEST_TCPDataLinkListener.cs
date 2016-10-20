using Networking;
using NUnit.Framework;
using System.Net.Sockets;
using System.Threading;

[TestFixture]
public class TEST_TCPDataLinkListener
{
    private TestIncomingDataLinkSubscriber _subscriber;
    private TCPDataLinkListener<ProtoBufPresentation> _listener;
    private TcpClient _incomingClient;

    [Test]
    public void AcceptConnection()
    {
        _subscriber = new TestIncomingDataLinkSubscriber();

        _listener = new TCPDataLinkListener<ProtoBufPresentation>(_subscriber);

        Assert.True(_listener.Start("127.0.0.1", 1234));

        _incomingClient = new TcpClient();
        _incomingClient.Connect("127.0.0.1", 1234);

        int sleepCount = 0;

        while (!_subscriber.Connected)
        {
            if (sleepCount > 50)
            {
                Assert.True(false, "Failed to connect within timeout.");
                break;
            }

            Thread.Sleep(1);

            sleepCount++;
        }

        Assert.IsTrue(_subscriber.Connected);
        Assert.NotNull(_subscriber.DataLink);
        Assert.IsTrue(_subscriber.DataLink.Connected());
    }

    [TearDown]
    public void Cleanup()
    {
        _incomingClient.Close();
        _subscriber.Cleanup();
        _listener.Stop();
    }
}

public class TestIncomingDataLinkSubscriber : IIncomingDataLinkSubscriber
{
    public volatile bool Connected = false;

    public volatile IDataLink DataLink;

    public void IncomingNewDataLink(IDataLink dataLink, IPresentationProtocol usedProtocol)
    {
        this.DataLink = dataLink;
        Connected = true;
    }

    public void Cleanup()
    {
        if (this.DataLink != null)
        {
            this.DataLink.Dispose();
        }
    }
}
