using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Configurable Properties")] [Tooltip("The movement Speed of the camera.")]
    public float movementSpeed;
    [Tooltip("This is the offset of the focal point. 0 means the camera is looking at the ground.")]
    public float lookOffset;
    [Tooltip("The zoom speed for the camera.")]
    public float zoomSpeed;
    [Tooltip("The default amount the player is zoomed into the world")]
    public float defaultZoom;
    [Tooltip("The most a player can zoom into the game world")]
    public float zoomMax;
    [Tooltip("The furthest point a player can zoom back from the game world")]
    public float zoomMin;
    [Tooltip("The point at which the camera should rotate upwards instead of moving backwards")]
    public float zoomRotationActivate;
    [Tooltip("The angle that the camera has when zoomed in over the activation area.")]
    public float minAngle;    
    [Tooltip("The angle that the camera has if zoomed out completely.")]
    public float maxAngle;
    [Tooltip("How fast the camera rotates")]
    public float rotationSpeed;

    private Camera _actualCamera;
    private Vector3 _cameraPositionTarget;
    
    private const float InternalMoveSpeed = 8;
    private Vector3 _moveTarget;
    private Vector3 _moveDirection;

    private float _internalZoomSpeed = 4;
    private float _currentZoomAmount;
    public float CurrentZoom
    {
        get => _currentZoomAmount;
        private set
        {
            _currentZoomAmount = value;
            UpdateCameraTarget();
        }
    }

    private float _currentRotationAmount;

    private bool _rightMouseDown = false;
    private const float InternalRotationSpeed = 4;
    private Quaternion _rotationTarget;
    private Vector2 _mouseDelta;

    private void UpdateCameraTarget()
    {
        if (_currentZoomAmount >= zoomRotationActivate) return;
        _cameraPositionTarget = (Vector3.up * lookOffset) +
                                (Quaternion.AngleAxis(minAngle, Vector3.right) * Vector3.back) * _currentZoomAmount;
    }

    private void Start()
    {
        _actualCamera = GetComponentInChildren<Camera>();
        
        Transform cameraTransform = _actualCamera.transform;
        cameraTransform.rotation = Quaternion.AngleAxis(minAngle, Vector3.right);
        CurrentZoom = defaultZoom;
        cameraTransform.position = _cameraPositionTarget;

        _rotationTarget = transform.rotation;

        _currentRotationAmount = minAngle;
    }

    private void FixedUpdate()
    {
        Transform rig = transform;
        _moveTarget += (rig.forward * _moveDirection.z + rig.right * _moveDirection.x) * (Time.fixedDeltaTime * movementSpeed);
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _moveTarget, Time.deltaTime * InternalMoveSpeed);

        _actualCamera.transform.localPosition = Vector3.Lerp(_actualCamera.transform.localPosition,
            _cameraPositionTarget, Time.deltaTime * _internalZoomSpeed);

        _actualCamera.transform.rotation = Quaternion.Euler(_currentRotationAmount, 0f, 0f);

        _rotationTarget *= Quaternion.AngleAxis(_mouseDelta.x * Time.deltaTime * rotationSpeed, Vector3.up);

        transform.rotation =
            Quaternion.Slerp(transform.rotation, _rotationTarget, Time.deltaTime * InternalRotationSpeed);
    }
    
    //Gets invoked from Player Input Script
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        _moveDirection = new Vector3(value.x, 0, value.y);
    }
    
    //Gets invoked from Player Input Script
    public void OnZoom(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed) return;

        CurrentZoom = Mathf.Clamp(_currentZoomAmount - ctx.ReadValue<Vector2>().y * zoomSpeed, zoomMax, zoomMin);
        
        float rotationLevel = Mathf.Clamp((_currentZoomAmount - zoomRotationActivate) / (zoomMin - zoomRotationActivate), 0, 1);
        _currentRotationAmount = rotationLevel * (maxAngle - minAngle) + minAngle;
    }
    
    //Gets invoked from Player Input Script
    public void OnRotateToggle(InputAction.CallbackContext ctx)
    {
        _rightMouseDown = ctx.ReadValue<float>() == 1f;
    }

    //Gets invoked from Player Input Script
    public void OnRotate(InputAction.CallbackContext ctx)
    {
        _mouseDelta = _rightMouseDown ? ctx.ReadValue<Vector2>() : Vector2.zero;
    }
}
