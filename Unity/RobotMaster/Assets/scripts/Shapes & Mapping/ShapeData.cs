using System;
using UnityEngine;
using System.Collections;

public class ShapeData
{

    public Mesh _mesh;

    public ShapeData(Mesh mesh)
    {
        if (mesh == null || mesh.vertices.Length <= 0)
        {
            throw new ArgumentException("shapedata not allowed to be empty", "mesh");
        }
        _mesh = mesh;
    }
}
