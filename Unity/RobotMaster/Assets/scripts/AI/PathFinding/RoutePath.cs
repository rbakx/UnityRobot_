using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoutePath : MonoBehaviour
{

    //private List<Vector3> waypoints;
     
	// Use this for initialization
	void Start ()
	{
	    //waypoints = new List<Vector3>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddWaypoint(Vector3 point)
    {
        
    }

    public bool RemoveWaypoint(Vector3 point)
    {
        return false;
    }

    public Vector3[] GetWaypointsInRange(Vector3 point)
    {
        Vector3[] pointsInRange = null;
        return pointsInRange;
    }
}
