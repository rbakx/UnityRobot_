using UnityEngine;
using System.Collections;
using Communication;
using Networking;

public class DummyReceiver : IMessageReceiver
{
    public Message incomingMessage;

    public void IncomingMessage(Message incomingMessage, IDataLink datalink)
    {
        this.incomingMessage = incomingMessage;
    }

    public bool SendCommand(Message message)
    {
        return true;
    }
}
