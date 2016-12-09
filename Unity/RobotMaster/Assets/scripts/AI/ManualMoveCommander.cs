using UnityEngine;
using System.Collections;


//script for handling individual commands
public class ManualMoveCommander : MonoBehaviour
{
    [SerializeField]
    private GameObject myRobot;

    public UnitSelecter USelecter;

    public void SetRobot(GameObject robot)
    {
        myRobot = robot;
    }

    public GameObject GetRobot()
    {
        return myRobot;
    }

    void Start()
    {
        USelecter = GameObject.Find("UnitControl").GetComponent<UnitSelecter>();
    }

    //add individual commands

    public void DestinationBtnPressed()
    {
        if (myRobot != null)
        {
            Debug.Log("MyRobot: " + myRobot.GetComponent<Robot>().GetRobotName());
            //select destination by raycast
            if (USelecter != null)
            {
                Debug.Log("Select Destination");
                USelecter.DestinationSelected = false;
                USelecter.selectingDestination = true;
               // USelecter.DestinationSelecter(myRobot);
            }
            else
            {
                Debug.Log("No unit selecter...");
            }
            //rotate towards destination
            //move forward until destination is reached
            //later on replace with navmesh
        }
        else
        {
            Debug.Log("No Robot bound to panel");
        }
    }

}
