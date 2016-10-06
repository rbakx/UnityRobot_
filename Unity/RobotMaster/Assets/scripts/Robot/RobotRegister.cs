using UnityEngine;
using System.Collections;
using Networking;
using Communication;
using System;

public class RobotRegister : IMessageReceiver
{
    private RobotList _robotList;

    public void IncomingMessage(Message newMessage, IDataLink dataLink)
    {
        
    }
}
