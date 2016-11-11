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

    public void UpdateMesh(List<Vector2> surfacePlain)
    {
        //Vector2[] _uvs;
        int[] _triangles;

        CalculateSurfaceTriangles(surfacePlain, out _triangles);

        Vector3[] points = ShiftPlane(surfacePlain, ref _triangles, Height);

        ApplyMesh(points, _triangles);
    }

    /*
        ShiftPlane converts a 3d surface to a 3D object with a defined height
    */
    private Vector3[] ShiftPlane(List<Vector2> _2dPlane, ref int[] indicesArray, float height)
    {
        Vector3[] shifted = new Vector3[_2dPlane.Count * 2];

        // Duplicate the vertices
        for (int i = 0; i < _2dPlane.Count; i++)
        {
            shifted[i] = new Vector3(_2dPlane[i].x, height, _2dPlane[i].y);
            shifted[i + _2dPlane.Count] = new Vector3(_2dPlane[i].x, 0.0F, _2dPlane[i].y);
        }

        // Duplicate the triangles
        int[] triangles = new int[indicesArray.Length * 2];

        //Copy the indexes for the top plane to triangles
        Array.Copy(indicesArray, triangles, indicesArray.Length);

        for (int i = 0; i < indicesArray.Length; i++)
        {
            triangles[i + indicesArray.Length] = indicesArray[i] + _2dPlane.Count;
        }

        // Invert the triangles of the bottom face
        Array.Reverse(triangles, indicesArray.Length, indicesArray.Length);

        // New array with the length of the current triangles array (this includes the up face), 
        // plus room for all side face triangles
        indicesArray = new int[triangles.Length + _2dPlane.Count * 6];
        Array.Copy(triangles, indicesArray, triangles.Length);

        int currTriangleIndex = triangles.Length;

        // Generate the side faces

        //Draw a triangle from top to bottom to the point in question
        for (int upperSurfaceIndex = 0; upperSurfaceIndex < _2dPlane.Count; upperSurfaceIndex++)
        {
            int nextUpperSurfaceIndex = upperSurfaceIndex + 1;

            if (nextUpperSurfaceIndex >= _2dPlane.Count)
                { nextUpperSurfaceIndex = 0; }

            int lowerSurfaceIndex = upperSurfaceIndex + _2dPlane.Count;
            int nextLowerSurfaceIndex = lowerSurfaceIndex + 1;

            if (nextLowerSurfaceIndex >= _2dPlane.Count*2)
                { nextLowerSurfaceIndex = _2dPlane.Count; }

            indicesArray[currTriangleIndex++] = (nextLowerSurfaceIndex);
            indicesArray[currTriangleIndex++] = (nextUpperSurfaceIndex);
            indicesArray[currTriangleIndex++] = (upperSurfaceIndex);

            indicesArray[currTriangleIndex++] = (nextLowerSurfaceIndex);
            indicesArray[currTriangleIndex++] = (upperSurfaceIndex);
            indicesArray[currTriangleIndex++] = (lowerSurfaceIndex);

        }

        return shifted;
    }

    private void CalculateSurfaceTriangles(List<Vector2> points, out int[] triangles)
    {
        Triangulator.GenerateSurfaceTriangles(points, out triangles);
        //UVs = Triangulator.BuildUVs(points);
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

    private void ApplyMesh(Vector3[] vertices, int[] triangles)
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
