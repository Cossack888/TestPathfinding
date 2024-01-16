using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Unit : MonoBehaviour
{
    public OverlayTile CurrentTile { get; private set; }
    public OverlayTile TargetTile { get; private set; }
    public bool isFollowing;
    public Identity Id { get; private set; }
    public MoveAction MoveAction { get; private set; }

    private void Start()
    {

        MoveAction = GetComponent<MoveAction>();
    }
    public void SetCurrentTile(OverlayTile tile)
    {
        CurrentTile = tile;
    }
    public void SetTargetTile(OverlayTile tile)
    {
        TargetTile = tile;
    }
    public void ResetTargetTile()
    {
        TargetTile = null;
    }

    public void SetUpIdentity(Identity IdToSet)
    {
        Id = IdToSet;
    }
}
