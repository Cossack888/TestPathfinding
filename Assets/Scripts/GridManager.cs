using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    [SerializeField] LayerMask unwalkableMask;
    [SerializeField] LayerMask floorMask;
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius;
    OverlayTile[,] grid;
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    public List<OverlayTile> Tiles { get; private set; }
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Tiles = new List<OverlayTile>();
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public void DestroyGrid()
    {
        List<OverlayTile> tilesCopy = new List<OverlayTile>(TilesManager.Instance.GetAllTiles());

        foreach (OverlayTile tile in tilesCopy)
        {
            if (tile != null && tile.gameObject != null)
            {
                TilesManager.Instance.RemoveTileFromAllTiles(tile);
                tile.gameObject.SetActive(false);
            }
        }
    }

    public void CreateGrid()
    {
        Unit currentUnit = UnitSelectionManager.Instance.GetCurrentUnit();
        Vector3 basePosition = currentUnit.transform.position;
        Vector3 worldBottomLeft = new Vector3(basePosition.x, 0.05f, basePosition.z) - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        grid = new OverlayTile[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, 0.7f, unwalkableMask));
                bool isOnFloor = (Physics.CheckSphere(worldPoint, 0.5f, floorMask));

                if (isOnFloor)
                {
                    OverlayTile tile = GetInactiveTile();
                    if (tile == null)
                    {
                        Transform gridTile = Instantiate(gridSystemVisualSinglePrefab, worldPoint, Quaternion.identity, gameObject.transform);
                        tile = gridTile.GetComponent<OverlayTile>();
                        Tiles.Add(tile);
                    }

                    tile.gameObject.SetActive(true);
                    tile.name = worldPoint.ToString();
                    tile.SetTile(walkable, worldPoint, x, y);
                    tile.ResetColor();
                    tile.transform.position = worldPoint;
                    if (!tile.Walkable)
                    {
                        tile.SetColor(Color.red);
                    }

                    TilesManager.Instance.AddTileToAllTiles(tile);
                    TilesManager.Instance.SetTilesMap(worldPoint, tile);
                    grid[x, y] = tile;
                }
            }
        }
    }

    private OverlayTile GetInactiveTile()
    {
        foreach (OverlayTile tile in Tiles)
        {
            if (!tile.gameObject.activeSelf)
            {
                return tile;
            }
        }
        return null;
    }


    public List<OverlayTile> GetNeighbours(OverlayTile node)
    {
        List<OverlayTile> neighbours = new List<OverlayTile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {

                if (x == 0 && y == 0)
                    continue;
                int checkX = node.GridX + x;
                int checkY = node.GridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }


    public OverlayTile NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }
}
