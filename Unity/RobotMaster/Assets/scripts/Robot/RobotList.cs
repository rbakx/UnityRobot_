using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RobotList : MonoBehaviour
{

    private List<Robot> _robots;

    public RobotList()
    {
        _robots = new List<Robot>();
    }

    void start()
    {

    }

    IEnumerator CheckIfStillConnected()
    {
        while(true)
        {
            for(int i = (_robots.Count-1); i >=0 ;i--)
            {
                Robot r = _robots[i];

                if(!r.IsConnected())
                {
                    _robots.Remove(r);

                    GameObject.Destroy(r.gameObject, 1.0F);
                    Debug.Log("[RobotList]: robot got disconnected! " + r.name + " [" + r.GetRobotType() + "]");
                }
            }

            yield return new WaitForSeconds(0.5F);
        }
    }

    public void Add(Robot robot)
    {
        if(robot != null && !_robots.Contains(robot))
        {
            _robots.Add(robot);
        }
    }

    public void Remove(Robot robot)
    {
        _robots.Remove(robot);
    }

    public Robot Get(int index)
    {
       return _robots[index];
    }

    public int Count
    {
        get
        { return _robots.Count; }
    }
}
