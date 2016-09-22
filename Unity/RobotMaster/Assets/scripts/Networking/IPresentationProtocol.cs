using UnityEngine;
using System.Collections;

public interface IPresentationProtocol
{
	
	
	void IncomingData(string data);
	
	string MessageToBinaryData(messsage : Message);
	
	Message BinaryDataToMessage(string data);
}
 