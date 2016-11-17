using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PointsToModelBuilder : MonoBehaviour
{
    public float Height = 1.0f;
	public Material MeshMaterial;

    private Mesh _mesh;
	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;
	private MeshCollider _meshCollider;

	void Awake()
	{
		_meshFilter = GetComponent<MeshFilter>();
		_meshRenderer = GetComponent<MeshRenderer>();

		_meshCollider = gameObject.AddComponent<MeshCollider>();

		_meshRenderer.material = MeshMaterial;
	}

    void Start()
    {
        CreateMesh();     
    }

    public void UpdateMesh(List<Vector3> surfacePlain)
    {
        Vector3[] points = surfacePlain.ToArray();

        Vector3[] _vertices;
        Vector2[] _uvs;
        int[] _triangles;

        CalculateSurfaceTriangles(points, out _vertices, out _triangles, out _uvs);

        _vertices = ShiftPlane(_vertices, ref _triangles, Height);

        ApplyMesh(_vertices, _uvs, _triangles);
    }

    /*
        ShiftPlane converts a 3d surface to a 3D object with a defined height
    */
    private Vector3[] ShiftPlane(Vector3[] _2dPlane, ref int[] indicesArray, float height)
    {
        Vector3[] shifted = new Vector3[_2dPlane.Length * 2];

        // Duplicate the vertices
        for (int i = 0; i < _2dPlane.Length; i++)
        {
            shifted[i] = _2dPlane[i];
            shifted[i + _2dPlane.Length] = _2dPlane[i] + new Vector3(0.0F, height, 0.0F);
        }

        // Duplicate the triangles
        int[] triangles = new int[indicesArray.Length * 2];
        for (int i = 0; i < indicesArray.Length; i++)
        {
            triangles[i] = indicesArray[i];
            triangles[i + indicesArray.Length] = indicesArray[i] + _2dPlane.Length;
        }

        // Invert the triangles of the bottom face
        Array.Reverse(triangles, 0, indicesArray.Length);

        // New array with the length of the current triangles array (this includes the up face), 
        // plus room for all side face triangles
        indicesArray = new int[triangles.Length + ((_2dPlane.Length + 1) * 6)];
        Array.Copy(triangles, indicesArray, triangles.Length);

        int currTriangleIndex = triangles.Length;

        // Generate the side faces
        for (int upperSurfaceIndex = 0; upperSurfaceIndex < _2dPlane.Length; upperSurfaceIndex++)
        {
            int nextUpperSurfaceIndex = upperSurfaceIndex + 1;

            if (nextUpperSurfaceIndex >= _2dPlane.Length)
            {
                nextUpperSurfaceIndex = 1;
            }

            int lowerSurfaceIndex = upperSurfaceIndex + _2dPlane.Length;
            int nextLowerSurfaceIndex = lowerSurfaceIndex + 1;

            if (nextLowerSurfaceIndex >= _2dPlane.Length*2)
            {
                nextLowerSurfaceIndex = _2dPlane.Length + 1;
            }

            indicesArray[currTriangleIndex++] = (upperSurfaceIndex);
            indicesArray[currTriangleIndex++] = (nextUpperSurfaceIndex);
            indicesArray[currTriangleIndex++] = (nextLowerSurfaceIndex);

            indicesArray[currTriangleIndex++] = (lowerSurfaceIndex);
            indicesArray[currTriangleIndex++] = (upperSurfaceIndex);
            indicesArray[currTriangleIndex++] = (nextLowerSurfaceIndex);

        }

        return shifted;
    }

    private void CalculateSurfaceTriangles(Vector3[] points, out Vector3[] vertices, out int[] triangles, out Vector2[] UVs)
    {
        Triangulator.GenerateSurfaceTriangles(points, out vertices, out triangles);
        UVs = Triangulator.BuildUVs(vertices);
    }


    private void CreateMesh()
    {
        _mesh = new Mesh();

		if (_meshFilter== null)//Will never happen with requiredcomponent
        {
            Debug.LogWarning("A mesh filter is required");
            return;
        }
        _mesh.name = "Generated mesh";
		_meshFilter.mesh = _mesh;

        MeshRenderer mr = GetComponent<MeshRenderer>();

        if (mr == null)//Will never happen
        {
            Debug.LogWarning("A mesh renderer is required");
            return;
        }
    }

    private void ApplyMesh(Vector3[] vertices, Vector2[] uv_mapping_surface, int[] triangles)
    {
		_mesh.triangles = null;
		_mesh.vertices = null;

        _mesh.vertices = vertices;
		_mesh.triangles = triangles;
        //_mesh.uv = uv_mapping_surface;

		_mesh.RecalculateNormals();
		_mesh.RecalculateBounds();

		_meshFilter.mesh = _mesh;
		_meshCollider.sharedMesh = _mesh;
    }
}
