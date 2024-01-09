using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float zoomSpeed;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.CameraMovementValue;

        float moveSpeed = 10f;
        Vector3 position = transform.position;
        Vector3 moveVector = transform.up * inputMoveDir.y + transform.right * inputMoveDir.x;
        position += moveVector * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(position.x, 5, position.z);

    }
    private void HandleZoom()
    {
        float camSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        camSize += InputManager.Instance.CameraZoomAmount * Time.deltaTime * zoomSpeed;
        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(camSize, 5, 20);
    }


}