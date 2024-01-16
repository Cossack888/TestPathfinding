using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        if (RaycastSent() != null && RaycastSent().TryGetComponent<Unit>(out Unit unit))
        {
            UnitSelectionManager.Instance.SelectUnit(unit);
        }
    }

    public void RightClick()
    {
        Unit currentUnit = UnitSelectionManager.Instance.GetCurrentUnit();
        GameObject clickableObject = RaycastSent();

        if (currentUnit != null)
        {
            if (clickableObject != null && clickableObject.TryGetComponent<OverlayTile>(out OverlayTile tile))
            {
                currentUnit.SetTargetTile(tile);
                currentUnit.MoveAction.MoveToTile(tile);
                ChooseNeighboringTile(currentUnit);
            }
            else
            {
                OverlayTile nearestTile = CheckForNearestTile();

                currentUnit.SetTargetTile(nearestTile);
                currentUnit.MoveAction.MoveToTile(nearestTile);
                ChooseNeighboringTile(currentUnit);
            }
        }
    }

    public void ChooseNeighboringTile(Unit currentUnit)
    {
        List<OverlayTile> tilesList = GridManager.Instance.GetNeighbours(currentUnit.TargetTile);

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (unit != currentUnit)
            {
                OverlayTile chosenTile = ChooseRandomTile(tilesList);
                if (chosenTile != null)
                {
                    unit.SetTargetTile(chosenTile);
                    StartCoroutine(Follow(1.5f, unit, chosenTile));
                }
                else
                {
                    Debug.LogWarning("No more available tiles to assign.");
                    break;
                }
            }
        }
    }

    public OverlayTile ChooseRandomTile(List<OverlayTile> tilesList)
    {
        if (tilesList.Count > 0)
        {
            return tilesList[Random.Range(0, tilesList.Count)];
        }
        else
        {
            Debug.LogWarning("No more available tiles to choose.");
            return null;
        }
    }

    IEnumerator Follow(float waitTime, Unit unit, OverlayTile tile)
    {
        yield return new WaitForSeconds(waitTime);
        unit.MoveAction.MoveToTile(tile);
    }

    public Vector3 GetTarget()
    {
        return targetPosition;
    }

    public OverlayTile GetNearestTile()
    {
        OverlayTile nearestTile = null;
        float nearestDistance = Mathf.Infinity;

        foreach (OverlayTile tile in GridManager.Instance.Tiles)
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
                foreach (OverlayTile tile in GridManager.Instance.Tiles)
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
