using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance { get; private set; }
    private Camera _mainCamera;
    private RaycastHit _hit;
    [SerializeField] LayerMask ObjectsLayer;
    void Start()
    {
        _mainCamera = Camera.main;
        InputManager.Instance.LeftClickEvent += LeftClick;
        InputManager.Instance.RightClickEvent += RightClick;
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one ClickManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void LeftClick()
    {
        UnitSelectionManager.Instance.SelectUnit(RaycastSent().GetComponent<Unit>());
    }
    public void RightClick()
    {
        Unit currentUnit = UnitSelectionManager.Instance.GetCurrentUnit();
        GameObject clickableObject = RaycastSent();
        if (currentUnit != null && clickableObject != null)
        {
            currentUnit.GetComponent<MoveAction>().Move(clickableObject.transform.position);
        }

    }

    public GameObject RaycastSent()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 2000f, ObjectsLayer))
            {
                GameObject go = _hit.collider.gameObject;
                return go;
            }
        }
        return null;
    }

}
