using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public OverlayTile currentTile { get; private set; }
    public OverlayTile targetTile { get; private set; }

    public float mobility;


    public MoveAction moveAction { get; private set; }


    private void Start()
    {
        mobility = Random.Range(1, 5);
        moveAction = GetComponent<MoveAction>();
    }
    public void SetCurrentTile(OverlayTile tile)
    {
        currentTile = tile;
    }
    public void SetTargetTile(OverlayTile tile)
    {
        targetTile = tile;
    }
    public void ResetTargetTile()
    {
        targetTile = null;
    }
    public void MoveToTargetTile()
    {
        StartCoroutine(TryToGetToTargetTile(2f));
    }
    IEnumerator TryToGetToTargetTile(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (moveAction.Velocity < 0.1f && currentTile != targetTile && currentTile != null)
        {
            // ClickManager.Instance.ChooseNeighboringTile(UnitSelectionManager.Instance.currentUnit);
            moveAction.MoveToTile(this.targetTile);
            StartCoroutine(TryToGetToTargetTile(2f));
        }
    }

}
