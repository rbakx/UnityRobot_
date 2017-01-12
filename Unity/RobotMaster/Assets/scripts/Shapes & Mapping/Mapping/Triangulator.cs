using UnityEngine;
using System.Collections.Generic;

public class Triangulator
{
    static public void GenerateSurfaceTriangles(List<Vector2> _points, out int[] triangleIndexes)
    {
        List<int> indices = new List<int>();

        int currentVertexPositionInArray = _points.Count;
        if (currentVertexPositionInArray < 3)
        {
            triangleIndexes = indices.ToArray();
            return;
        }

        int[] verticeIndex = new int[currentVertexPositionInArray];

        //If area is negative, reverse the plane
        // If it isn't negative, setup a basic array
        if (Area(_points) >= 0)
        {
            for (int v = 0; v < currentVertexPositionInArray; v++)
                verticeIndex[v] = v;
        }
        else
        {
            for (int v = 0; v < currentVertexPositionInArray; v++)
                verticeIndex[v] = (currentVertexPositionInArray - 1) - v;
        }

        int count = 2 * currentVertexPositionInArray;

        for (int m = 0, v = currentVertexPositionInArray - 1; currentVertexPositionInArray > 2;)
        {
            if ((count--) <= 0)
            {
                triangleIndexes = indices.ToArray();
                break;
            }

            int u = v;
            if (currentVertexPositionInArray <= u)
                { u = 0; }

            v = u + 1;

            if (currentVertexPositionInArray <= v)
                { v = 0; }

            int w = v + 1;

            if (currentVertexPositionInArray <= w)
                { w = 0; }

            if (CheckToSnip(_points, u, v, w, currentVertexPositionInArray, verticeIndex))
            {
                int a, b, c, s, t;
                a = verticeIndex[u];
                b = verticeIndex[v];
                c = verticeIndex[w];

                indices.Add(a);
                indices.Add(b);
                indices.Add(c);

                m++;

                for (s = v, t = v + 1; t < currentVertexPositionInArray; s++, t++)
                    { verticeIndex[s] = verticeIndex[t]; }

                currentVertexPositionInArray--;
                count = 2 * currentVertexPositionInArray;
            }
        }

        indices.Reverse();
        triangleIndexes = indices.ToArray();
    }

    // Calculate the total area of the object
    static private float Area(List<Vector2> _points)
    {
        int n = _points.Count;
        float A = 0.0F;

        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = _points[p];
            Vector2 qval = _points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }
        return (A * 0.5F);
    }

    static private bool CheckToSnip(List<Vector2> _points, int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = _points[V[u]];
        Vector2 B = _points[V[v]];
        Vector2 C = _points[V[w]];


        //Check if surface is not negative and also not really really small
        if ((((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))) <= Mathf.Epsilon)
            { return false; } 

        for (p = 0; p < n; p++)
        {
            // The target point is not a point of the given triangle..
            if ((p == u) || (p == v) || (p == w))
                { continue; }

            // Get the point position
            Vector2 P = _points[V[p]];

            // This point is inside the given triangle, so this may not be snipped.
            if (InsideTriangle(A, B, C, P))
                { return false; }
        }

        return true;
    }

    /*
        Check if point is within a triangle
    */
    static private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}