using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PointsToModelBuilder))]
public class DebugPointsToModelBuilder : MonoBehaviour
{

    public List<Vector2> surface;
    public bool UpdateModel = false;

	void Start()
	{
		StartCoroutine("CheckForUpdateModel");
	}

    IEnumerator CheckForUpdateModel()
    {
        while (true)
        {
            if (UpdateModel)
            {
                PointsToModelBuilder builder = GetComponent<PointsToModelBuilder>();

                List<Vector2> _sf = surface;

                List<Vector3> SurfaceV3 = new List<Vector3>(_sf.Count);

                foreach (Vector2 val in _sf)
                {
                    SurfaceV3.Add(new Vector3(val.x, 0.0F, val.y));
                }

                builder.UpdateMesh(SurfaceV3);
                UpdateModel = false;
            }

            yield return new WaitForSeconds(0.1F);
        }
    }


    public void SetShape1()
    {
        surface = new List<Vector2>();
        surface.Add(new Vector2(0, 3));
        surface.Add(new Vector2(3, 5));
        surface.Add(new Vector2(6, 3));
        surface.Add(new Vector2(4.5f, 0));
        surface.Add(new Vector2(3,-1));
        surface.Add(new Vector2(1.5f, 0));
        UpdateModel = true;
    }

    public void SetShape2()
    {
        surface = new List<Vector2>();
        surface.Add(new Vector2(0, 3));
        surface.Add(new Vector2(2f, 3));
        surface.Add(new Vector2(3, 5));
        surface.Add(new Vector2(4f, 3));
        surface.Add(new Vector2(6, 3));
        surface.Add(new Vector2(4, 1.5f));
        surface.Add(new Vector2(4.5f, 0));
        surface.Add(new Vector2(3, 1));
        surface.Add(new Vector2(1.5f, 0));
        surface.Add(new Vector2(2, 2));

        UpdateModel = true;
    }
}

