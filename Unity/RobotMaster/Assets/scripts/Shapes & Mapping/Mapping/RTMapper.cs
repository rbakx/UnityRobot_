using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RTMapper : MonoBehaviour
{
    public float Height = 2.0f;
    public Vector3[] ReceivedData;

    private Vector3[] _shiftedVertices;
    private int[] _triangles;

    private Mesh _mesh;
    private bool _meshChanged = true;

    void Start()
    {
        CreateMesh();

        List<Vector3> incoming = new List<Vector3>();

        incoming.Add(new Vector3(0, 0, 0));
        incoming.Add(new Vector3(1, 0, 0));
        incoming.Add(new Vector3(0.5f, 0, 0.5f));
        incoming.Add(new Vector3(1, 0, 1));
        incoming.Add(new Vector3(0, 0, 1));
        incoming.Add(new Vector3(0.2f, 0, 0.5f));

        ReceivedData = incoming.ToArray();
    }

    void Update()
    {
        if (_meshChanged)
        {
            _triangles = CalculateTriangles(ReceivedData);
            _shiftedVertices = ShiftPlane(ReceivedData);
            ApplyMesh();
            _meshChanged = false;
        }
    }

    private Vector3[] ShiftPlane(Vector3[] _2dPlane)
    {
        Vector3[] shifted = new Vector3[_2dPlane.Length * 2];

        // Duplicate the vertices
        for (int i = 0; i < _2dPlane.Length; i++)
        {
            shifted[i] = _2dPlane[i];
            shifted[i + _2dPlane.Length] = _2dPlane[i] + new Vector3(0, Height, 0);
        }

        // Duplicate the triangles
        int[] triangles = new int[_triangles.Length * 2];
        for (int i = 0; i < _triangles.Length; i++)
        {
            triangles[i] = _triangles[i];
            triangles[i + _triangles.Length] = _triangles[i] + _2dPlane.Length;
        }

        // Invert the triangles of the bottom face
        Array.Reverse(triangles, 0, _triangles.Length);

        // New array with the length of the current triangles array (this includes the up face), 
        // plus room for all side face triangles
        _triangles = new int[triangles.Length + ((_2dPlane.Length + 1) * 6)];
        Array.Copy(triangles, _triangles, triangles.Length);

        int currTriangleIndex = triangles.Length;

        // Generate the side faces
        for (int i = 0; i < _2dPlane.Length; i++)
        {
            int nextIndex = i + 1;
            if (nextIndex >= _2dPlane.Length)
            {
                nextIndex = 0;
            }

            int upIndex = i + _2dPlane.Length;
            int nextUpIndex = upIndex + 1;
            if (nextUpIndex >= shifted.Length)
            {
                nextUpIndex = _2dPlane.Length;
            }

            _triangles[currTriangleIndex++] = (nextUpIndex);
            _triangles[currTriangleIndex++] = (nextIndex);
            _triangles[currTriangleIndex++] = (i);

            _triangles[currTriangleIndex++] = (nextUpIndex);
            _triangles[currTriangleIndex++] = (i);
            _triangles[currTriangleIndex++] = (upIndex);

        }

        return shifted;
    }

    private int[] CalculateTriangles(Vector3[] vertices)
    {
        Vector2[] points = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            points[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        Triangulator triangulator = new Triangulator(points);
        return triangulator.Triangulate();
    }

    private void CreateMesh()
    {
        _mesh = new Mesh();
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.LogWarning("A mesh filter is required");
            return;
        }
        _mesh.name = "Generated mesh";
        mf.mesh = _mesh;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
        {
            Debug.LogWarning("A mesh renderer is required");
            return;
        }
    }

    private void ApplyMesh()
    {
        _mesh.vertices = _shiftedVertices;
        _mesh.triangles = _triangles;

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }
}
