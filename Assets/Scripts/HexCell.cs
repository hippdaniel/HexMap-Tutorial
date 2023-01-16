using System;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public RectTransform uiRect;
    public Color color;
    
    private int _elevation;
    public int Elevation
    {
        get => _elevation;
        set
        {
            _elevation = value;
            
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.ElevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = _elevation * -HexMetrics.ElevationStep;
            uiRect.localPosition = uiPosition;
        }
    }

    [SerializeField] private HexCell[] neighbors = new HexCell[6];

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
    
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(_elevation, neighbors[(int)direction].Elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(_elevation, otherCell.Elevation);
    }
}
