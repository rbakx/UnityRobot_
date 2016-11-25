using UnityEngine;
using System.Collections;
using UnityEditor;

public class UnitSelecter : MonoBehaviour
{

    Ray ray;
    private RaycastHit hit;

    public bool selected;
    public bool selecting;
    public bool RobotFound = false;
    public GameObject SelectedUnit;

    public UIController UIControl;

    public LayerMask unitLayer;
    public LayerMask terrainLayer;

    void Start()
    {
        selected = false;
        selecting = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && selecting)
        {
            LookforTarget();
        }
        if (selected && Input.GetKeyDown(KeyCode.Escape))
        {
            selected = false;
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
        if (!selected)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer) && hit.transform.tag == "Unit")
            {

                hit.collider.gameObject.GetComponent<RobotSelect>().isSelected = true;
                selected = true;
                SelectedUnit = hit.collider.gameObject;
                RobotFound = true;
                UIControl.robotFound = true;
                return SelectedUnit;
            }

        }
        /*else
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer))
            {
                Vector3 point = hit.point;
                Debug.Log("World point " + point);
                MoveUnit(SelectedUnit, point);
                selected = false;
                SelectedUnit.GetComponent<RobotSelect>().isSelected = false;
                SelectedUnit = null;
            }
        }*/
        Debug.DrawRay(ray.origin, ray.direction, Color.black);
        return null;
    }

    void MoveUnit(GameObject unit, Vector3 p)
    {
        unit.gameObject.GetComponent<Robot>().SetLinearVelocity(p);
    }
}
