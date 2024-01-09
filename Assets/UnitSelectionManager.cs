using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    Unit currentUnit;
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
    }
    public void SelectUnit(Unit unit)
    {
        SetCurrentUnit(unit);
    }
    public void SetCurrentUnit(Unit unit)
    {
        currentUnit = unit;
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