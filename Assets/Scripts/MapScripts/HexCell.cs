using System;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public RectTransform uiRect;
    public HexGridChunk chunk;
    
    [SerializeField] private HexCell[] neighbors = new HexCell[6];
    [SerializeField] private bool[] roads;
    
    private bool _hasIncomingRiver, _hasOutgoingRiver;
    public bool HasIncomingRiver => _hasIncomingRiver;
    public bool HasOutgoingRiver => _hasOutgoingRiver;
    private HexDirection _incomingRiver, _outgoingRiver;
    public HexDirection IncomingRiver => _incomingRiver;
    public HexDirection OutgoingRiver => _outgoingRiver;
    public bool HasRiver => _hasIncomingRiver || _hasOutgoingRiver;
    public bool HasRiverBeginOrEnd => _hasIncomingRiver != _hasOutgoingRiver;
    public float StreamBedBy => (_elevation + HexMetrics.StreamBedElevationOffset) * HexMetrics.ElevationStep;
    public float RiverSurfaceY => (_elevation + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;
    public float WaterSurfaceY => (_waterLevel + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;
    public bool HasRoads
    {
        get
        {
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i]) return true;
            }

            return false;
        }
    }
    public HexDirection RiverBeginOrEndDirection => _hasIncomingRiver ? _incomingRiver : _outgoingRiver;

    private Color _color;
    public Color Color
    {
        get => _color;
        set
        {
            if (_color == value) return;
            _color = value;
            Refresh();
        }
    }

    private int _elevation = int.MinValue;
    public int Elevation
    {
        get => _elevation;
        set
        {
            if (_elevation == value) return;
            _elevation = value;
            
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.ElevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.ElevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;

            ValidateRivers();

            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }
            
            Refresh();
        }
    }
    public Vector3 Position => transform.position;

    private int _waterLevel;
    public int WaterLevel
    {
        get => _waterLevel;
        set
        {
            if (_waterLevel == value) return;
            
            _waterLevel = value;
            ValidateRivers();
            Refresh();
        }
    }
    public bool IsUnderwater => _waterLevel > _elevation;
    
    private void Refresh()
    {
        if(chunk) chunk.Refresh();
        for (int i = 0; i < neighbors.Length; i++) {
            HexCell neighbor = neighbors[i];
            if (neighbor != null && neighbor.chunk != chunk) {
                neighbor.chunk.Refresh();
            }
        }
    }

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

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return _hasIncomingRiver && _incomingRiver == direction || _hasOutgoingRiver && _outgoingRiver == direction;
    }
    
    public bool IsValidRiverDestination(HexCell neighbor)
    {
        return neighbor && (_elevation >= neighbor._elevation || _waterLevel == neighbor._elevation);
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if(_hasOutgoingRiver && _outgoingRiver == direction) return;

        HexCell neighbor = GetNeighbor(direction);
        if (!IsValidRiverDestination(neighbor)) return;
        
        RemoveOutgoingRiver();
        if (_hasIncomingRiver && _incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        _hasOutgoingRiver = true;
        _outgoingRiver = direction;
        
        neighbor.RemoveIncomingRiver();
        neighbor._hasIncomingRiver = true;
        neighbor._incomingRiver = direction.Opposite();
        
        SetRoad((int)direction, false);
    }

    public void RemoveOutgoingRiver()
    {
        if (!_hasOutgoingRiver) return;
        _hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(_outgoingRiver);
        neighbor._hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveIncomingRiver()
    {
        if (!_hasIncomingRiver) return;
        _hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(_incomingRiver);
        neighbor._hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public void AddRoad(HexDirection direction)
    {
        if (!roads[(int)direction] && !HasRiverThroughEdge(direction) && GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    private void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
        neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    public void RemoveRoads()
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (roads[i]) roads[i] = false;
            RefreshSelfOnly();
            if (neighbors[i])
            {
                neighbors[i].roads[(int)((HexDirection)i).Opposite()] = false;
                neighbors[i].RefreshSelfOnly();
            }
        }
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = _elevation - GetNeighbor(direction)._elevation;
        return difference >= 0 ? difference : -difference;
    }
    
    private void ValidateRivers()
    {
        if (_hasOutgoingRiver && !IsValidRiverDestination(GetNeighbor(_outgoingRiver)))
        {
            RemoveOutgoingRiver();
        }

        if (_hasIncomingRiver && !GetNeighbor(_incomingRiver).IsValidRiverDestination(this))
        {
            RemoveIncomingRiver();
        }
    }
    
}
