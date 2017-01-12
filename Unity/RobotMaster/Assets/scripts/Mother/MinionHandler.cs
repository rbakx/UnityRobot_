using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MinionHandler : MonoBehaviour
{

    public List<GameObject> RobotList;
    public string Message;
    public string thisMsg;

    public GameObject Nao;
    public GameObject Mindstorm;

    public Vector3 Location;
    public Quaternion Rotation;

    private GameObject newRobot;

    void Start()
    {
        RobotList = new List<GameObject>();
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
    public void SetMsg(string msg)
    {
        Message = msg;
        if(HandleMsg(Message))
        {
            StartCoroutine(SleepNAdd(0.5f,"Hanz"));
            
        }
    }

    //in case of instantiate not ready, wait half second
    IEnumerator SleepNAdd(float seconds, string name)
    {
        yield return new WaitForSeconds(seconds);
        AddRobotToList(newRobot, name);
    }

    //add new robot to list
    void AddRobotToList(GameObject robot, string name)
    {
        robot.GetComponent<Robot>().SetName(name);

        RobotList.Add(robot);
    }

    //handle incoming message, return true if msg was succesfully handled
    public bool HandleMsg(string message)
    {
        //add new robot (params: name, type, location)
        if (message == "Register Nao")
        {
            Location = new Vector3(0, 0, 0);
            Rotation = new Quaternion(0, 0, 0, 0);
            newRobot = (GameObject)Instantiate(Nao, Location, Rotation );
            return true;

        }
        return false;
    }      
}

