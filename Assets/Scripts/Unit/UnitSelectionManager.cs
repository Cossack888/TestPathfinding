using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public Unit currentUnit;
    public Unit[] units;
    public MoveAction sourceUnitMoveAction;
    public MoveAction destinationUnitMoveAction;
    public bool gridWasReset;
    public static UnitSelectionManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitSelectionManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        units = FindObjectsOfType<Unit>();
    }
    public bool AreAnyUnitsMoving()
    {
        return units.Any(unit => unit.moveAction.Velocity > 0.1f);
    }
    public void SelectUnit(Unit unit)
    {
        SetCurrentUnit(unit);
    }
    public void SetCurrentUnit(Unit unit)
    {

        currentUnit = unit;

        ResetGrid();

    }
    /*private void Update()
    {
        if (!AreAnyUnitsMoving() && !gridWasReset && currentUnit != null)
        {
            ResetGrid();
            gridWasReset = true;
        }
    }*/
    public void ResetGrid()
    {
        GridManager grid = GridManager.Instance;
        foreach (Unit tempUnit in FindObjectsOfType<Unit>())
        {
            tempUnit.moveAction.path.Clear();
            tempUnit.ResetTargetTile();
        }
        grid.DestroyGrid();
        grid.CreateGrid();

    }
    public Unit GetCurrentUnit()
    {
        return currentUnit;
    }
    public void ClearCache()
    {
        currentUnit = null;
    }
}
