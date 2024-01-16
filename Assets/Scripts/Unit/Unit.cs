using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public OverlayTile currentTile { get; private set; }
    public OverlayTile targetTile { get; private set; }
    public float mobility;
    public bool isFollowing;
    public Identity Id { get; private set; }
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
    public void SetUpIdentity(Identity IdToSet)
    {
        Id = IdToSet;
    }
}
