using UnityEngine;
using System.Collections;

public class Robot : MonoBehaviour
{
    private string Name;
    private string type;
    private Vector3 location;


	// Use this for initialization
	void Start ()
	{
	    Name = "";
	    type = "";
	    location = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetName(string name)
    {
        Name = name;
    }
}
