using UnityEngine;
using System.Collections;

public class VerticesToMap : MonoBehaviour
{
    void Update()
    {
    }

    public Vector2[] V2E;
    public Mesh mesh;

    public void VerticesToObjects(Vector2[] verticesToExtrude) { 
    mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int i = 0;
        while (i < vertices.Length)
        {
            for(int j = 0; j < verticesToExtrude.Length; j++)
            {
                if(vertices[i].x == verticesToExtrude[j].x && vertices[i].z == verticesToExtrude[j].y && verticesToExtrude[j] != new Vector2(-1,-1))
                {
                    vertices[i].y += 100;
                    verticesToExtrude[j] = new Vector2(-1, -1);
                }
                j++;
            }
            i++;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}

