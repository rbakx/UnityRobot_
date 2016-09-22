using UnityEngine;
using System.Collections;

public class Communication : MonoBehaviour {
    
   public Communication(IMessageReceiver receiver)
    {
        //;
    }

    public bool SendCommand(Message msg)
    {
        return false;
    }
}
