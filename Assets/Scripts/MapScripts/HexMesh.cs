using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    public bool useCollider, useColors, useUvCoordinates, useUv2Coordinates;

    private Mesh _hexMesh;
    private MeshCollider _meshCollider;
    [NonSerialized] private List<Vector3> _vertices;
    [NonSerialized] private List<int> _triangles;
    [NonSerialized] private List<Color> _colors;
    [NonSerialized] private List<Vector2> _uvs;
    [NonSerialized] private List<Vector2> _uv2s;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
        if(useCollider) _meshCollider = gameObject.AddComponent<MeshCollider>();
        _hexMesh.name = "Hex Mesh";
    }

    public void Clear()
    {
        _hexMesh.Clear();
        _vertices = ListPool<Vector3>.Get();
        _triangles = ListPool<int>.Get();
        if(useColors) _colors = ListPool<Color>.Get();
        if(useUvCoordinates) _uvs = ListPool<Vector2>.Get();
        if (useUv2Coordinates) _uv2s = ListPool<Vector2>.Get();
    }

    public void Apply()
    {
        _hexMesh.SetVertices(_vertices);
        ListPool<Vector3>.Add(_vertices);
        
        if (useColors)
        {
            _hexMesh.SetColors(_colors);
            ListPool<Color>.Add(_colors);
        }

        if (useUvCoordinates)
        {
            _hexMesh.SetUVs(0, _uvs);
            ListPool<Vector2>.Add(_uvs);
        }

        if (useUv2Coordinates)
        {
            _hexMesh.SetUVs(1, _uv2s);
            ListPool<Vector2>.Add(_uv2s);
        }
        
        _hexMesh.SetTriangles(_triangles, 0);
        ListPool<int>.Add(_triangles);
        
        _hexMesh.RecalculateNormals();
        if(useCollider) _meshCollider.sharedMesh = _hexMesh;
    }
    
    public void AddTriangle(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(HexMetrics.Perturb(vec1));
        _vertices.Add(HexMetrics.Perturb(vec2));
        _vertices.Add(HexMetrics.Perturb(vec3));
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }

    public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }
    
    public void AddTriangleColor(Color color)
    {
        _colors.Add(color);
        _colors.Add(color);
        _colors.Add(color);
    }

    public void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        _colors.Add(c1);
        _colors.Add(c2);
        _colors.Add(c3);
    }
    
    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        _uvs.Add(uv1);
        _uvs.Add(uv2);
        _uvs.Add(uv3);
    }
    
    public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        _uv2s.Add(uv1);
        _uv2s.Add(uv2);
        _uv2s.Add(uv3);
    }

    public void AddQuad(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(HexMetrics.Perturb(vec1));
        _vertices.Add(HexMetrics.Perturb(vec2));
        _vertices.Add(HexMetrics.Perturb(vec3));
        _vertices.Add(HexMetrics.Perturb(vec4));
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 3);
    }
    
    public void AddQuadUnperturbed (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _vertices.Add(v4);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 3);
    }

    public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        _colors.Add(c1);
        _colors.Add(c2);
        _colors.Add(c3);
        _colors.Add(c4);
    }
    
    public void AddQuadColor(Color c1, Color c2)
    {
        _colors.Add(c1);
        _colors.Add(c1);
        _colors.Add(c2);
        _colors.Add(c2);
    }

    public void AddQuadColor(Color c)
    {
        _colors.Add(c);
        _colors.Add(c);
        _colors.Add(c);
        _colors.Add(c);
    }

    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
    {
        _uvs.Add(uv1);
        _uvs.Add(uv2);
        _uvs.Add(uv3);
        _uvs.Add(uv4);
    }

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        _uvs.Add(new Vector2(uMin, vMin));
        _uvs.Add(new Vector2(uMax, vMin));
        _uvs.Add(new Vector2(uMin, vMax));
        _uvs.Add(new Vector2(uMax, vMax));
    }
    
    public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector2 uv3, Vector2 uv4)
    {
        _uv2s.Add(uv1);
        _uv2s.Add(uv2);
        _uv2s.Add(uv3);
        _uv2s.Add(uv4);
    }

    public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
    {
        _uv2s.Add(new Vector2(uMin, vMin));
        _uv2s.Add(new Vector2(uMax, vMin));
        _uv2s.Add(new Vector2(uMin, vMax));
        _uv2s.Add(new Vector2(uMax, vMax));
    }
}
