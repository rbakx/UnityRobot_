using UnityEngine;
using System.Collections;
using Networking;

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
