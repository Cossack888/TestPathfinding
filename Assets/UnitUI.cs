using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    public GameObject portraitPrefab;
    private void Start()
    {
        SetUpPortraits();
    }
    public void SetUpPortraits()
    {
        foreach (Unit unit in UnitSelectionManager.Instance.units)
        {
            GameObject portrait = Instantiate(portraitPrefab, transform.position, transform.rotation, gameObject.transform);
            portrait.GetComponentInChildren<Image>().sprite = unit.Id.Face;
            portrait.GetComponentInChildren<TMP_Text>().text = unit.Id.unitName;
            portrait.GetComponent<IndividualUI>().ConnectUnitToUI(unit);
        }
    }


}