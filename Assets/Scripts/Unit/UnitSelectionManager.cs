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
    public Identity[] Id;
    [SerializeField] GameObject barbarianPrefab;
    [SerializeField] GameObject warriorPrefab;
    [SerializeField] GameObject paladinPrefab;
    [SerializeField] GameObject thiefPrefab;
    [SerializeField] Transform[] spawnLocation;
    [SerializeField] UnitUI unitUI;
    public static UnitSelectionManager Instance
    { get; private set; }

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
    private void Start()
    {
        for (int i = 0; i < Id.Length && i < spawnLocation.Length; i++)
        {


            switch (Id[i].unitBodyType)
            {
                case Identity.TypeOfUnit.Paladin:
                    GameObject paladinPref = Instantiate(paladinPrefab, spawnLocation[i].position, transform.rotation);
                    paladinPref.GetComponent<Unit>().SetUpIdentity(Id[i]);
                    break;
                case Identity.TypeOfUnit.Thief:
                    GameObject thiefPref = Instantiate(thiefPrefab, spawnLocation[i].position, transform.rotation);
                    thiefPref.GetComponent<Unit>().SetUpIdentity(Id[i]); break;
                case Identity.TypeOfUnit.Warrior:
                    GameObject warriorPref = Instantiate(warriorPrefab, spawnLocation[i].position, transform.rotation);
                    warriorPref.GetComponent<Unit>().SetUpIdentity(Id[i]); break;
                case Identity.TypeOfUnit.Barbarian:
                    GameObject barbarianPref = Instantiate(barbarianPrefab, spawnLocation[i].position, transform.rotation);
                    barbarianPref.GetComponent<Unit>().SetUpIdentity(Id[i]); break;


            }

        }
        units = FindObjectsOfType<Unit>();
        unitUI.SetUpPortraits();
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
        unit.isFollowing = false;
        foreach (Unit separateUnit in units)
        {
            if (separateUnit != unit)
            {
                unit.isFollowing = true;
            }

        }
        ResetGrid();

    }
    private void Update()
    {
        if (!AreAnyUnitsMoving() && !gridWasReset && currentUnit != null)
        {
            ResetGrid();
            gridWasReset = true;
        }
    }
    public void ResetGrid()
    {
        GridManager grid = GridManager.Instance;

        grid.DestroyGrid();
        grid.CreateGrid();

        foreach (Unit tempUnit in FindObjectsOfType<Unit>())
        {
            tempUnit.moveAction.path.Clear();
            tempUnit.ResetTargetTile();
        }
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
