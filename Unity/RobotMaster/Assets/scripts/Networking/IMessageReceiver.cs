using UnityEngine;
using System.Collections;

public interface IMessageReceiver {

	void IncomingMessage(Message newMessage);
}
