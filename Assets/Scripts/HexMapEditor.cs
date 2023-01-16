using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HexMapEditor : MonoBehaviour
{
    public InputAction onClick;
    
    public Color[] colors;
    public HexGrid hexGrid;
    public Color activeColor;

    private int _activeElevation;

    private bool _mouseOnUI;

    private void Awake()
    {
        SelectColor(0);
    }

    private void Update()
    {
        _mouseOnUI = EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId);
    }

    private void OnEnable()
    {
        onClick.Enable();
    }

    private void OnDisable()
    {
        onClick.Disable();
    }

    private void Start()
    {
        onClick.performed += _ => HandleInput();
    }

    private void HandleInput()
    {
        if (_mouseOnUI || Camera.main == null) return;
        
        Ray inputRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(inputRay, out RaycastHit hit))
        {
            EditCell(hexGrid.GetCell(hit.point));
        }
    }

    private void EditCell(HexCell cell)
    {
        cell.color = activeColor;
        cell.Elevation = _activeElevation;
        hexGrid.Refresh();
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }
    
    public void SetElevation(float elevation)
    {
        _activeElevation = (int)elevation;
    }
        
}
