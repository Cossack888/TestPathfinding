using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    [SerializeField] GameObject portraitPrefab;
    public void SetUpPortraits()
    {
        foreach (Unit unit in UnitSelectionManager.Instance.Units)
        {
            GameObject portrait = Instantiate(portraitPrefab, transform.position, transform.rotation, gameObject.transform);
            portrait.GetComponentInChildren<Image>().sprite = unit.Id.Face;
            portrait.GetComponentInChildren<TMP_Text>().text = unit.Id.unitName;
            portrait.GetComponent<IndividualUI>().ConnectUnitToUI(unit);
        }
    }


}