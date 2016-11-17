using System;
using UnityEngine;

public class RecognisedShape
{
    private Mesh _mesh;
    private Vector3 _position;
    private Quaternion _rotation;

    public Mesh Mesh
    {
        get { return _mesh; }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException("Mesh");
            }
            _mesh = value;
        }
    }

    public Vector3 Position
    {
        get { return _position; }
        set { _position = value; }
    }

    public Quaternion Rotation
    {
        get { return _rotation; }
        set { _rotation = value; }
    }

    public float MaxWidth { get { return _mesh.bounds.max.x; } }

    public float MaxHeight { get { return _mesh.bounds.max.y; } }

    public RecognisedShape(Mesh mesh, Vector3 position, Quaternion rotation)
    {
        if (mesh == null)
        {
            throw new ArgumentException("mesh");
        }

        _mesh = mesh;
        _position = position;
        _rotation = rotation;
    }

    public RecognisedShape(Mesh mesh) :
        this(mesh, Vector3.zero, Quaternion.identity)
    {

    }
}
