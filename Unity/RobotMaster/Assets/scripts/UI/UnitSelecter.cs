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
    }

    void LookforTarget()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 direction = this.transform.TransformDirection(Vector3.forward);
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
                Debug.Log("nothing selected");
                // Vector3 point = ray.origin + (ray.direction * 500);
                Vector3 point = hit.point;
                Debug.Log("World point " + point);
                StartCoroutine(Mover(point,0.2f));
                selected = false;
                SelectedUnit.GetComponent<RobotSelect>().isSelected = false;
                SelectedUnit = null;
            }
        }
        Debug.DrawRay(ray.origin, ray.direction, Color.black);
    }

    IEnumerator Mover(Vector3 p, float step)
    {
        while (SelectedUnit.transform.position != p)
        {
            SelectedUnit.transform.position = Vector3.MoveTowards(SelectedUnit.transform.position, p,
                step*Time.deltaTime);
        }
        yield return new WaitForSeconds(0f);
    }
}
