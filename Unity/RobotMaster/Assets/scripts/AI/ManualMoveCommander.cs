using UnityEngine;
using System.Collections;


//script for handling individual commands
public class ManualMoveCommander : MonoBehaviour
{
    [SerializeField]
    private GameObject myRobot;
    private Robot robotScript;

    public UnitSelecter USelecter;

    public void SetRobot(GameObject robot)
    {
        robotScript = robot.GetComponent<Robot>();

        if(robotScript == null)
        {
            throw new System.Exception("[ManualMoveCommander] SetRobot - GameObject must contain Robot script!");
        }

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
	
    public Robot GetRobotScript()
    {
        return robotScript;
    }

    public void Move()
    {
        Move(50.0F);
    }

    public void Move(float x = 0.0F, float y = 0.0F, float z = 0.0F)
    {
        robotScript.SetLinearVelocity(new Vector3(x, y, z));
    }

    public void CancelMove()
    {
        robotScript.StopMoving();
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
