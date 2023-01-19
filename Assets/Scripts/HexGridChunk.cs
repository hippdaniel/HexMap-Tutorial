using System;
using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{
    private HexCell[] _cells;
    private HexMesh _hexMesh;
    private Canvas _gridCanvas;

    private void Awake()
    {
        _gridCanvas = GetComponentInChildren<Canvas>();
        _hexMesh = GetComponentInChildren<HexMesh>();

        _cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
    }

    private void Start()
    {
        _hexMesh.TriangulateMap(_cells);
    }
    
    public void AddCell(int index, HexCell cell)
    {
        _cells[index] = cell;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(_gridCanvas.transform, false);
    }
}
