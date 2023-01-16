using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HexGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;
    public TMP_Text cellLabelPrefab;
    public Canvas gridCanvas;
    public HexMesh hexMesh;
    
    public Color defaultColor = Color.white;

    private HexCell[] _cells;

    private void Awake()
    {
        _cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - (float)Math.Floor(z / 2f)) * (HexMetrics.InnerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.OuterRadius * 1.5f);
        
        HexCell cell = _cells[i] = Instantiate<HexCell>(cellPrefab, transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
        }

        if (z > 0)
        {
            //& checks in bit. if z has a 1 as last bit (and is therefore uneven), the equation of z & 1 will be 1 and not 0
            if ((z & 1) == 0) 
            {
                cell.SetNeighbor(HexDirection.SE, _cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - width - 1]);
                }
            }
            else 
            {
                cell.SetNeighbor(HexDirection.SW, _cells[i - width]);
                if (x < width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - width + 1]);
                }
            }
        }
        
        TMP_Text label = Instantiate<TMP_Text>(cellLabelPrefab, gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;
    }

    private void Start()
    {
        hexMesh.TriangulateMap(_cells);
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        return _cells[index];
    }

    public void Refresh()
    {
        hexMesh.TriangulateMap(_cells);
    }
    
    
}
