using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, PlayerControls.IUnitsActions
{
    public Vector2 CameraMovementValue { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public float CameraZoomAmount { get; private set; }
    public Vector3 LeftClickPosition { get; private set; }
    public Vector3 RightClickPosition { get; private set; }
    public event Action LeftClickEvent;
    public event Action RightClickEvent;
    private PlayerControls controls;
    public static InputManager Instance { get; private set; }
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        controls = new PlayerControls();
        controls.Units.SetCallbacks(this);
        controls.Units.Enable();
    }


    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (context.performed) { LeftClickEvent?.Invoke(); }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed) { RightClickEvent?.Invoke(); }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }

    public void OnCameraMovement(InputAction.CallbackContext context)
    {
        CameraMovementValue = context.ReadValue<Vector2>();
    }

    public void OnCameraZoom(InputAction.CallbackContext context)
    {
        CameraZoomAmount = context.ReadValue<float>();
    }
}
