using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;


public class HexMapEditor : MonoBehaviour
{
    public InputAction onClick;
    
    public Color[] colors;
    public HexGrid hexGrid;
    public Color activeColor;

    private int _activeElevation;
    private int _brushSize;

    private bool _mouseOnUI;
    private bool _applyColor;
    private bool _applyElevation;
    private Camera _mainCam;
    private bool _isMainCamNull;

    private bool _isDrag;
    private HexDirection _dragDirection;
    private HexCell _previousCell;
    
    private enum OptionalToggle
    {
        Ignore, No, Yes
    }

    private OptionalToggle riverMode;

    private void Awake()
    {
        SelectColor(-1);
        _mainCam = Camera.main;
        _isMainCamNull = _mainCam == null;
    }

    private void Update()
    {
        _mouseOnUI = EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId);

        if (onClick.IsPressed()) HandleInput();
        else _previousCell = null;
    }

    private void OnEnable()
    {
        onClick.Enable();
    }

    private void OnDisable()
    {
        onClick.Disable();
    }

    private void HandleInput()
    {
        if (_mouseOnUI || _isMainCamNull)
        {
            _previousCell = null;
            return;
        }
        
        Ray inputRay = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(inputRay, out RaycastHit hit))
        {
            HexCell currentCell = hexGrid.GetCell(hit.point);
            if (_previousCell && _previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                _isDrag = false;
            }
            EditCells(currentCell);
            _previousCell = currentCell;
        }
        else
        {
            _previousCell = null;
        }
    }

    private void ValidateDrag(HexCell currentCell)
    {
        for (_dragDirection = HexDirection.NE; _dragDirection <= HexDirection.NW; _dragDirection++)
        {
            if (_previousCell.GetNeighbor(_dragDirection) == currentCell)
            {
                _isDrag = true;
                return;
            }
        }

        _isDrag = false;
    }

    private void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + _brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        
        for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - _brushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    private void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (_applyColor)
            {
                cell.Color = activeColor;
            }

            if (_applyElevation)
            {
                cell.Elevation = _activeElevation;
            }

            if (riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            } else if (_isDrag && riverMode == OptionalToggle.Yes)
            {
                HexCell otherCell = cell.GetNeighbor(_dragDirection.Opposite());
                if(otherCell) otherCell.SetOutgoingRiver(_dragDirection);
                _previousCell.SetOutgoingRiver(_dragDirection);
            }
        }
    }

    public void SelectColor(int index)
    {
        _applyColor = index >= 0;
        if (_applyColor)
        {
            activeColor = colors[index];
        }
    }
    
    public void SetElevation(float elevation)
    {
        _activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        _applyElevation = toggle;
    }

    public void SetBrushSize(float size)
    {
        _brushSize = (int)size;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }
}
