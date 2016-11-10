using UnityEngine;
using System.Collections.Generic;

public class Triangulator
{
    /// 

    public static void GenerateSurfaceTriangles(Vector3[] points, out Vector3[] vertices, out int[] triangles)
    {
        vertices = null;
        triangles = null;

        if (points == null || points.Length < 3)
        {
            Debug.Log("Define 2D polygon in 'poly' in the the Inspector");
            return;
        }

        Vector3 center = FindCenter(points);

        vertices = new Vector3[points.Length + 1];
        vertices[0] = Vector3.zero;

        for (int i = 0; i < points.Length; i++)
        {
            vertices[i + 1] = points[i] - center;
        }

        triangles = new int[points.Length * 3];

        for (int i = 0; i < points.Length - 1; i++)
        {
            triangles[i * 3] = i + 2;
            triangles[i * 3 + 1] = 0;
            triangles[i * 3 + 2] = i + 1;
        }

        triangles[(points.Length - 1) * 3] = 1;
        triangles[(points.Length - 1) * 3 + 1] = 0;
        triangles[(points.Length - 1) * 3 + 2] = points.Length;
    }

    public static Vector3 FindCenter(Vector3[] points)
    {
        Vector3 center = Vector3.zero;
        foreach (Vector3 v3 in points)
        {
            center += v3;
        }
        return center / points.Length;
    }

    public static Vector2[] BuildUVs(Vector3[] vertices)
    {

        float xMin = Mathf.Infinity;
        float yMin = Mathf.Infinity;
        float xMax = -Mathf.Infinity;
        float yMax = -Mathf.Infinity;

        foreach (Vector3 v3 in vertices)
        {
            if (v3.x < xMin)
                xMin = v3.x;
            if (v3.y < yMin)
                yMin = v3.y;
            if (v3.x > xMax)
                xMax = v3.x;
            if (v3.y > yMax)
                yMax = v3.y;
        }

        float xRange = xMax - xMin;
        float yRange = yMax - yMin;

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i].x = (vertices[i].x - xMin) / xRange;
            uvs[i].y = (vertices[i].y - yMin) / yRange;

        }
        return uvs;
    }
}