using UnityEngine;

public class UnitSelecter : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;

    public bool RobotSelected;
    public bool DestinationSelected;
    public bool selectingRobot;
    public bool selectingDestination;
    public bool RobotFound = false;

    public GameObject SelectedUnit;
    public GameObject SelectedPanel;

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
          //  Debug.Log("Looking for Robots");
            LookforTarget();
        }
        if (Input.GetMouseButtonDown(0) && selectingDestination)
        {
           // Debug.Log("Selected Unit:" + SelectedUnit);
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
       // Debug.DrawRay(ray.origin, ray.direction, Color.black);
        return null;
    }

    public void MoveUnit(GameObject unit, Vector3 p)
    {
        unit.gameObject.GetComponent<Robot>().SetLinearVelocity(p);
    }

    public void DestinationSelecter(GameObject unit)
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!DestinationSelected)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
            {
                Vector3 point = hit.point;
                if (unit.gameObject.GetComponent<Robot>() != null)
                {
                    SelectedPanel.GetComponent<ManualMoveCommander>().SetDestination(point);
                    DestinationSelected = true;
                    selectingDestination = false;
                }
                else
                {
                    Debug.Log("Could not select destination, the object youre trying to move is no robot");
                }
                
                if (unit.gameObject.GetComponent<Robot>() && DestinationSelected)
                {
                    unit.GetComponent<RobotSelect>().isSelected = false;
                   // Debug.Log("Deselecting Robot");
                    RobotSelected = false;
                }
                else
                {
                    Debug.Log("Object is not selectable");
                }
            }
        }

        Debug.DrawRay(ray.origin, ray.direction, Color.black);
    }
}
