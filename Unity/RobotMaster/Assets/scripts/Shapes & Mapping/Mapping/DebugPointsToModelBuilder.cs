using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PointsToModelBuilder))]
public class DebugPointsToModelBuilder : MonoBehaviour
{

    public List<Vector2> surface;
    public bool UpdateModel = false;

    void Update()
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

                List<Vector3> SurfaceV3 = new List<Vector3>(surface.Count);

                foreach (Vector2 val in surface)
                {
                    SurfaceV3.Add(new Vector3(val.x, 0.0F, val.y));
                }

                builder.UpdateMesh(SurfaceV3);
                UpdateModel = false;
            }

            yield return new WaitForSeconds(0.1F);
        }
    }
}

