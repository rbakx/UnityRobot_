using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RobotList))]
public class MinionHandler : MonoBehaviour
{

    public RobotList RobotList;
    public string Message;
    public string thisMsg;

    public GameObject Nao;
    public GameObject Mindstorm;

    public Vector3 Location;
    public Quaternion Rotation;

    private GameObject newRobot;

    public string[] RobotNames;

    void Start()
    {
        RobotList = GetComponent<RobotList>();
        Message = "";
    }

    void Update()
    {

    }

    //TODO
    public bool ReadProtoBuf(string incomming)
    {

        return false;
    }

    //dummy code for testing messages
    /*public void SetMsg(string msg)
    {
        Message = msg;

        if (HandleMsg(Message))
        {
            int activeRobots = RobotList.Count;
            StartCoroutine(SleepNAdd(0.6f, RobotNames[activeRobots]));
        }
    }*/

    //handle incoming message, return true if msg was succesfully handled
    public bool HandleMsg(string message)
    {
        //add new robot (params: name, type, location)
        if (message == "Register Nao")
        {
            //testing purposes
            Vector3 position = new Vector3(Random.Range(-100.0f, 100.0f), 3, Random.Range(-100.0f, 100.0f));
            Location = position;
            Rotation = new Quaternion(0, 0, 0, 0);
            newRobot = (GameObject)Instantiate(Nao, Location, Rotation);

            return true;

        }
        return false;
    }
}

