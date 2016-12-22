using UnityEngine;
using System.Collections;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;


//script for handling individual commands
public class ManualMoveCommander : MonoBehaviour
{
    [SerializeField]
    private GameObject myRobot;

    public GameObject DestinationLabel;

    public GameObject RobotTypelabel;

    private Robot robotScript;

    public UnitSelecter USelecter;

    public Vector3 Destination;

    public void SetDestination(Vector3 point)
    {
        DestinationLabel.GetComponent<Text>().text = "Destination: " + point.ToString();
        Destination = point;
        
        myRobot.GetComponent<Robot>().SetDestination(point);
    }

    void LateUpdate()
    {
        const float _robot_forward_speed = 30.0F;
        // const float _robot_forward_speed = 0.0F;
        
        if (myRobot == null)
        {
            Destroy(this);
            return;
        }

        Vector3 targetDir = Destination - myRobot.transform.position;

        float distance = Vector3.Distance(myRobot.transform.position, Destination);

        if (distance > 50.5F)
        {
           // Debug.Log("distance: " + distance);

            targetDir.y = 0.0F;

            myRobot.transform.position += myRobot.transform.forward*Time.smoothDeltaTime* _robot_forward_speed;

            Vector3 newDir = Vector3.RotateTowards(transform.up, targetDir, Mathf.Infinity, 0.0F);
            Debug.DrawRay(transform.position, newDir, Color.red);
            myRobot.transform.rotation = Quaternion.Lerp(myRobot.transform.rotation, Quaternion.LookRotation(newDir), 0.2f);
        }
    }

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
        RobotTypelabel.GetComponent<Text>().text = "Robottype: " + robotScript.GetRobotType();
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
        //    Debug.Log("MyRobot: " + myRobot.GetComponent<Robot>().GetRobotName());
            //select destination by raycast
            if (USelecter != null)
            {
                USelecter.SelectedUnit = myRobot;
                USelecter.SelectedPanel = this.gameObject;
             //   Debug.Log("Select Destination");
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
