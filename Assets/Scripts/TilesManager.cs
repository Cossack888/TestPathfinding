using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TilesManager : MonoBehaviour
{
    public static TilesManager Instance { get; private set; }
    List<OverlayTile> allTiles;
    Dictionary<Vector3, OverlayTile> tilesMap;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TilesManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        allTiles = new List<OverlayTile>();
        tilesMap = new Dictionary<Vector3, OverlayTile>();
    }

    public void RemoveObstacleTiles(OverlayTile tile)
    {
        tile.walkable = true;
        tile.ResetColor();

    }
    public void SetTilesMap(Vector3 worldPoint, OverlayTile tile)
    {
        if (tilesMap.ContainsKey(worldPoint)) return;
        tilesMap.Add(worldPoint, tile);
    }

    public Dictionary<Vector3, OverlayTile> GetTilesMap()
    {
        return tilesMap;
    }

}
