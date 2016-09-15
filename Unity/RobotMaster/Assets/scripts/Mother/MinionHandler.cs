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

    public bool ReadProtoBuf(string incomming)
    {

        return false;
    }

    public void SetMsg(string msg)
    {
        Message = msg;
        if(HandleMsg(Message))
        {
            StartCoroutine("Sleeper");
            
        }
    }

    IEnumerator Sleeper()
    {
        yield return new WaitForSeconds(.5f);
        AddRobot(newRobot, "Hans");
    }

    void AddRobot(GameObject robot, string name)
    {
        robot.GetComponent<Robot>().SetName(name);

        RobotList.Add(robot);
    }
    public bool HandleMsg(string message)
    {
        //add new robot (params: name, type, location)
        if (message == "Register Nao")
        {
            Location = new Vector3(0, 0, 0);
            newRobot = (GameObject)Instantiate(Nao, Location, new Quaternion(0, 0, 0, 0));
            return true;

        }
        return false;
    }      
}

