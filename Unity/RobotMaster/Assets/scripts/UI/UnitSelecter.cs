using UnityEngine;
using System.Collections;
using UnityEditor;

public class UnitSelecter : MonoBehaviour
{

    Ray ray;
    private RaycastHit hit;

    private bool selected;
    public GameObject SelectedUnit;

    public LayerMask unitLayer;
    public LayerMask terrainLayer;

    void Start()
    {
        selected = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

    void LookforTarget()
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
            }

        }
        else
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
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.black);
    }

    void MoveUnit(GameObject unit, Vector3 p)
    {
        unit.gameObject.GetComponent<Robot>().SetLinearVelocity(p);
    }
}
