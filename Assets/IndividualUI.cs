using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndividualUI : MonoBehaviour
{
    public Unit Unit { get; private set; }

    public void ConnectUnitToUI(Unit unit)
    {
        Unit = unit;
    }
    public void WhenButtonIsClicked()
    {
        UnitSelectionManager.Instance.SelectUnit(Unit);
    }
}
