using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour, IHeapItem<OverlayTile>
{

    public bool walkable;
    public bool blocked;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    private int heapIndex;
    public Unit currentUnit;
    public OverlayTile parent;
    public MeshRenderer tempRenderer;
    public Color neutralColor;
    public OverlayTile SetTile(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        this.walkable = walkable;
        worldPosition = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
        return this;
    }
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    private void Start()
    {
        tempRenderer = GetComponent<MeshRenderer>();
    }
    public int CompareTo(OverlayTile nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit unit) && !blocked)
        {

            currentUnit = unit;
            unit.moveAction.MovedOneTile(this);
            unit.SetCurrentTile(this);
            blocked = true;
            if (unit.moveAction.path.Count == 0 && UnitSelectionManager.Instance.currentUnit == unit)
            {
                UnitSelectionManager.Instance.ResetGrid();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        /*if (other.TryGetComponent<Unit>(out Unit unit) && currentUnit == unit && unit.moveAction.path.Count == 0)
        {
            unit.moveAction.Move(transform.position);
        }*/
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit unit))
        {
            currentUnit = null;
            blocked = false;
        }
    }
    public void SetColor(Color color)
    {
        tempRenderer.material.color = color;
    }
    public void ResetColor()
    {
        tempRenderer.material.color = neutralColor;
    }

    public void SetColorTemp(Color color, float time)
    {
        tempRenderer.material.color = color;
        StartCoroutine(ResetTempTile(time));
    }
    private IEnumerator ResetTempTile(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        tempRenderer.material.color = neutralColor;

    }
    public Color GetColor() { return tempRenderer.material.color; }
}
