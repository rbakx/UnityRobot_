using UnityEngine;
using System.Collections;
using Networking;
using System;

public class LocalDataLink : IDataLink
{
    private IDataStreamReceiver _receiver;

    public bool Connected()
    {
        return true;
    }

    public void Dispose()
    {

    }

    public bool SendData(byte[] data)
    {
        return true;
    }

    public void SetReceiver(IDataStreamReceiver receiver)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException("receiver");
        }

        _receiver = receiver;
    }
}
