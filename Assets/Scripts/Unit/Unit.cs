using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public OverlayTile currentTile;
    public MoveAction moveAction { get; private set; }
    private void Start()
    {
        moveAction = GetComponent<MoveAction>();
    }
    public void SetCurrentTile(OverlayTile tile)
    {
        currentTile = tile;
    }

}
