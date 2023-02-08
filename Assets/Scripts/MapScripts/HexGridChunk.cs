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
        ShowUI(false);
    }

    public void AddCell(int index, HexCell cell)
    {
        _cells[index] = cell;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(_gridCanvas.transform, false);
        cell.chunk = this;
    }

    public void Refresh()
    {
        enabled = true;
    }

    public void LateUpdate()
    {
        _hexMesh.TriangulateMap(_cells);
        enabled = false;
    }

    public void ShowUI(bool visible)
    {
        _gridCanvas.gameObject.SetActive(visible);
    }
}
