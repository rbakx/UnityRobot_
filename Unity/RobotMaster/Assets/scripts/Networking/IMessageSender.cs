using UnityEngine;
using System.Collections;

public interface IMessageSender
{
	bool SendCommand(Message message);
}
