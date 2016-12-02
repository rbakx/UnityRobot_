using UnityEngine;
using System.Collections;


//script for handling individual commands
public class ManualMoveCommander : MonoBehaviour
{
    [SerializeField]
    private GameObject myRobot;
    private Robot robotScript;

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

    public Robot GetRobotScript()
    {
        return robotScript;
    }

    public void Move()
    {
        Move(10.0F);
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
}
