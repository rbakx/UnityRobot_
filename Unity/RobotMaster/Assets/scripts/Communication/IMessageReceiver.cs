using UnityEngine;
using System.Collections;

public interface IMessageReceiver
{

    void IncommingMessage(Message newMessage);

}
