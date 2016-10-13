using Networking;
using Communication;
using NUnit.Framework;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

[TestFixture]
public class TEST_TCPAsioConnection
{
    private TestIncomingDataLinkSubscriberCopy _subscriber;
    private TCPDataLinkListener<ProtoBufPresentation> _listener;


    [Test]
    public void AcceptConnection()
    {
		Debug.LogWarning("[TEST_TCPAsioConnection.AcceptConnection] This test will only pass when a tcp client is manually connected to the listener started in this test.");

        _subscriber = new TestIncomingDataLinkSubscriberCopy();

        _listener = new TCPDataLinkListener<ProtoBufPresentation>(_subscriber);
		//use public IP address not localhost/127.0.0.1
		//use eduroam(same network)
        Assert.True(_listener.Start("145.93.45.166", 1234));


        int sleepCount = 0;

        while (!_subscriber.Connected)
        {
            if (sleepCount > 500)
            {
                Assert.True(false, "Failed to connect within timeout.");
                break;
            }

            Thread.Sleep(20);

            sleepCount++;
        }
        Debug.Log("Actually connected");
		    //Thread.Sleep(8000);

        Assert.IsTrue(_subscriber.Connected);
        Assert.NotNull(_subscriber.DataLink);
        Assert.IsTrue(_subscriber.DataLink.Connected());

        if(_subscriber.Connected)
        {
            Thread.Sleep(3000);
        }
    }

    [TearDown]
    public void Cleanup()
    {
      //  _incomingClient.Close();
        _subscriber.Cleanup();
        _listener.Stop();
    }
}

public class TestIncomingDataLinkSubscriberCopy : IIncomingDataLinkSubscriber, IMessageReceiver
{
    public volatile bool Connected = false;

    public volatile IDataLink DataLink;

    public void IncomingNewDataLink(IDataLink dataLink, IPresentationProtocol usedProtocol)
    {
        this.DataLink = dataLink;
        usedProtocol.SetReceiver(this);
        Connected = true;
        Debug.Log("Incoming impl");
    }

	public void IncomingMessage(Message newMessage, IDataLink datalink)
	{
		Debug.Log(newMessage.stringData);
	}

    public void Cleanup()
    {
        if (this.DataLink != null)
        {
            this.DataLink.Dispose();
        }
    }
}
