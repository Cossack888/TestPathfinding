using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour, IHeapItem<OverlayTile>
{

    public bool Walkable { get; private set; }
    public bool Blocked { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public int GridX { get; private set; }
    public int GridY { get; private set; }
    public int GCost { get; private set; }
    public int HCost { get; private set; }
    private int heapIndex;
    public Unit CurrentUnit { get; private set; }
    public OverlayTile Parent { get; private set; }
    [SerializeField] MeshRenderer tempRenderer;
    public Color neutralColor;
    UnitSelectionManager manager;
    public OverlayTile SetTile(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        this.Walkable = walkable;
        WorldPosition = worldPos;
        this.GridX = gridX;
        this.GridY = gridY;
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
            return GCost + HCost;
        }
    }
    private void Start()
    {
        manager = UnitSelectionManager.Instance;
    }
    public int CompareTo(OverlayTile nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }
        return -compare;
    }
    private void OnMouseEnter()
    {
        if (Walkable) SetColor(Color.cyan);
    }
    private void OnMouseExit()
    {
        if (Walkable) ResetColor();
    }
    private void OnTriggerEnter(Collider other)
    {


        if ((other.TryGetComponent<Unit>(out Unit unit) && !Blocked && unit.TargetTile != this))
        {

            CurrentUnit = unit;
            unit.SetCurrentTile(this);
            Blocked = true;

        }
        else
        {
            if (unit.TargetTile == this)
            {
                manager.gridWasReset = false;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit unit))
        {
            CurrentUnit = null;
            Blocked = false;

        }
    }
    public void SetCosts(int gCost, int hCost, OverlayTile parent)
    {
        GCost = gCost;
        HCost = hCost;
        Parent = parent;
    }
    public void SetWalkable()
    {
        Walkable = true;
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
