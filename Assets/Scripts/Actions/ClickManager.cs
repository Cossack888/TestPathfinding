using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance { get; private set; }
    private Camera _mainCamera;
    private RaycastHit _hit;
    [SerializeField] LayerMask ObjectsLayer;
    public Vector3 targetPosition;
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

        if (currentUnit != null)
        {
            if (clickableObject != null && clickableObject.TryGetComponent<OverlayTile>(out OverlayTile tile))
            {
                currentUnit.moveAction.MoveToTile(tile);

                List<OverlayTile> tilesList = GridManager.Instance.GetNeighbours(tile);

                foreach (Unit unit in FindObjectsOfType<Unit>())
                {
                    Debug.Log($"{unit.name} Position: {unit.transform.position}, Target Position: {clickableObject.transform.position}");

                    if (unit != currentUnit && unit.moveAction.path.Count == 0)
                    {
                        if (tilesList.Count > 0)
                        {
                            OverlayTile chosenTile = tilesList[0];
                            tilesList.RemoveAt(0);
                            unit.targetTile = chosenTile;
                            StartCoroutine(Follow(1, unit, chosenTile));
                        }
                        else
                        {
                            Debug.LogWarning("No more available tiles to assign.");
                            break;
                        }
                    }
                }
            }
            else
            {
                currentUnit.moveAction.MoveToTile(CheckForNearestTile());
            }
        }
    }
    IEnumerator Follow(float waitTime, Unit unit, OverlayTile tile)
    {
        yield return new WaitForSeconds(waitTime);
        if (unit.moveAction.path.Count == 0)
        {
            unit.moveAction.MoveToTile(tile);
        }
    }

    public Vector3 GetTarget()
    {
        return targetPosition;
    }

    public OverlayTile GetNearestTile()
    {
        OverlayTile nearestTile = null;
        float nearestDistance = Mathf.Infinity;

        foreach (OverlayTile tile in GridManager.Instance.tiles)
        {
            float distance = Vector3.Distance(targetPosition, tile.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTile = tile;
            }
        }
        return nearestTile;
    }

    public OverlayTile CheckForNearestTile()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit, 2000f, ObjectsLayer))
            {
                Vector3 pos = _hit.point;
                targetPosition = pos;
                OverlayTile nearestTile = null;
                float nearestDistance = Mathf.Infinity;

                foreach (OverlayTile tile in GridManager.Instance.tiles)
                {
                    float distance = Vector3.Distance(pos, tile.transform.position);

                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestTile = tile;
                    }
                }
                return nearestTile;
            }
        }

        return null;
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
