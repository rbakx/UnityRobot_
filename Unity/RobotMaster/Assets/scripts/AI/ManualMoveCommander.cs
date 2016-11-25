using UnityEngine;
using System.Collections;


//script for handling individual commands
public class ManualMoveCommander : MonoBehaviour
{
    [SerializeField]
    private GameObject myRobot;

    public void SetRobot(GameObject robot)
    {
        myRobot = robot;
    }

    public GameObject GetRobot()
    {
        return myRobot;
    }

    //add individual commands
}
