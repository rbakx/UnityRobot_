using UnityEngine;
using System.Collections;
using Networking;

public class Robot : MonoBehaviour
{
    private string Name;
    private string Type;
    private Vector3 location;
    private Quaternion rotation;

    private Vector3 destination;

    private bool moving;

	void Start ()
	{
	    Name = "";
	    Type = "";
	    location = this.transform.position;
	    rotation = this.transform.rotation;
	}
	
	void Update () {

	    if (moving && location != destination)
	    {
	        transform.position = Vector3.MoveTowards(location, destination, 2f * Time.deltaTime);
	        location = transform.position;
            Debug.Log("hans is een steen");
	    }

	}

    public Robot(Communicator com)
    {
        
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetType(string type)
    {
        this.Type = type;
    }

    public string GetRobotName()
    {
        return this.name;
    }

    public string GetRobotType()
    {
        return this.Type;
    }

    public void MoveMe(Vector3 destiny)
    {
        moving = true;
        this.destination = destiny;
    }

    public bool GetMoving()
    {
        return moving;
    }

    public void StopMoving()
    {
        MoveMe(this.transform.position);
    }

    public void RotateMe(Direction d)
    {

    }

    public void indicate()
    {

    }
}
