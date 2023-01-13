using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    private Mesh _hexMesh;
    private MeshCollider _meshCollider;
    private List<Vector3> _vertices;
    private List<int> _triangles;

    private List<Color> _colors;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = _hexMesh = new Mesh();
        _meshCollider = gameObject.AddComponent<MeshCollider>();
        _hexMesh.name = "Hex Mesh";
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
        _colors = new List<Color>();
    }
    
    public void TriangulateMap(HexCell[] cells)
    {
        _hexMesh.Clear();
        _vertices.Clear();
        _triangles.Clear();
        _colors.Clear();

        foreach (var cell in cells)
        {
            TriangulateCell(cell);
        }

        _hexMesh.vertices = _vertices.ToArray();
        _hexMesh.triangles = _triangles.ToArray();
        _hexMesh.colors = _colors.ToArray();
        _hexMesh.RecalculateNormals();
        _meshCollider.sharedMesh = _hexMesh;
    }

    private void TriangulateCell(HexCell cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
        {
            TriangulateCell(d, cell);
        }
    }

    private void TriangulateCell(HexDirection direction, HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                center,
                center + HexMetrics.GetSolidCorner(direction,1),
                center + HexMetrics.GetSolidCorner(direction,2)
                );
            AddTriangleColor(cell.color);
        }
    }

    private void AddTriangleColor(Color color)
    {
        _colors.Add(color);
        _colors.Add(color);
        _colors.Add(color);
    }

    private void AddTriangle(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(vec1);
        _vertices.Add(vec2);
        _vertices.Add(vec3);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }

    private void AddQuad(Vector3 vec1, Vector3 vec2, Vector3 vec3, Vector3 vec4)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(vec1);
        _vertices.Add(vec2);
        _vertices.Add(vec3);
        _vertices.Add(vec4);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
        _triangles.Add(vertexIndex + 3);
    }
}
