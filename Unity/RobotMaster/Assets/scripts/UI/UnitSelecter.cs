using UnityEngine;
using System.Collections;
using UnityEditor;

public class UnitSelecter : MonoBehaviour
{

    Ray ray;
    private RaycastHit hit;

    public bool RobotSelected;
    public bool DestinationSelected;
    public bool selectingRobot;
    public bool selectingDestination;
    public bool RobotFound = false;

    public GameObject SelectedUnit;

    public UIController UIControl;

    public LayerMask unitLayer;
    public LayerMask terrainLayer;

    void Start()
    {
        RobotSelected = false;
        DestinationSelected = false;
        selectingRobot = false;
        selectingDestination = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && selectingRobot)
        {
            LookforTarget();
        }
        if (Input.GetMouseButtonDown(0) && selectingDestination)
        {
            Debug.Log(SelectedUnit);
            DestinationSelecter(SelectedUnit);
        }

        if (RobotSelected && Input.GetKeyDown(KeyCode.Escape))
        {
            RobotSelected = false;
            SelectedUnit.GetComponent<RobotSelect>().isSelected = false;
            if (SelectedUnit.GetComponent<Robot>().IsMoving())
            {
                MoveUnit(SelectedUnit, SelectedUnit.gameObject.transform.position);
            }
            SelectedUnit = null;
        }
    }

    public GameObject LookforTarget()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Vector3 direction = this.transform.TransformDirection(Vector3.forward);
        if (!RobotSelected)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer) && hit.transform.tag == "Unit")
            {

                hit.collider.gameObject.GetComponent<RobotSelect>().isSelected = true;
                RobotSelected = true;
                SelectedUnit = hit.collider.gameObject;
                RobotFound = true;
                UIControl.robotFound = true;
                return SelectedUnit;
            }

        }
        Debug.DrawRay(ray.origin, ray.direction, Color.black);
        return null;
    }

    public void MoveUnit(GameObject unit, Vector3 p)
    {
        unit.gameObject.GetComponent<Robot>().SetLinearVelocity(p);
    }

    public void DestinationSelecter(GameObject unit)
    {
        Debug.Log(DestinationSelected);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!DestinationSelected)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
            {
                Vector3 point = hit.point;
                Debug.Log("World point " + point);
                if (unit.gameObject.GetComponent<Robot>() != null)
                {
                    unit.gameObject.GetComponent<Robot>().SetDestination(point);
                    Debug.Log("We gaan weer golven!!");
                    DestinationSelected = true;
                    selectingDestination = false;
                }
                else
                {
                    Debug.Log("IS NO ROBOOOT!!");
                }
                
                if (unit.gameObject.GetComponent<Robot>() && DestinationSelected)
                {
                    unit.GetComponent<RobotSelect>().isSelected = false;
                    Debug.Log("Deselect Robot");
                    RobotSelected = false;
                }
                else
                {
                    Debug.Log("IS NOT SELECTABLE");
                }
            }

        }

        Debug.DrawRay(ray.origin, ray.direction, Color.black);
    }
}
