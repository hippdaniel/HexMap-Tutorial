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

    private void Awake()
    {
        SelectColor(-1);
        _mainCam = Camera.main;
        _isMainCamNull = _mainCam == null;
    }

    private void Update()
    {
        _mouseOnUI = EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId);
        
        if(onClick.IsPressed()) HandleInput();
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
        if (_mouseOnUI || _isMainCamNull) return;
        
        Ray inputRay = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(inputRay, out RaycastHit hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
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

}
