using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public LayerMask unwalkableMask;
    public LayerMask floorMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    OverlayTile[,] grid;
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    public List<OverlayTile> tiles;
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
        tiles = new List<OverlayTile>();
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
        if (tiles.Count > 0)
        {
            foreach (OverlayTile tile in tiles)
            {
                Destroy(tile.gameObject);
            }
        }
    }

    public void CreateGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Transform gridTile = Instantiate(gridSystemVisualSinglePrefab, transform.position, Quaternion.identity, gameObject.transform);
                tiles.Add(gridTile.GetComponent<OverlayTile>());
            }
        }
    }
    public void MoveGrid()
    {
        Vector3 basePosition = UnitSelectionManager.Instance.GetCurrentUnit().transform.position;
        Vector3 worldBottomLeft = new Vector3(basePosition.x, 0.05f, basePosition.z) - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        grid = new OverlayTile[gridSizeX, gridSizeY];

        int tileIndex = 0; // Keep track of the index of the tile in the list

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, 0.7f, unwalkableMask));
                bool isOnFloor = (Physics.CheckSphere(worldPoint, 0.5f, floorMask));

                if (isOnFloor)
                {
                    // Check if there are existing tiles
                    if (tileIndex < tiles.Count)
                    {
                        OverlayTile tile = tiles[tileIndex];
                        tileIndex++; // Move to the next tile in the list

                        tile.SetTile(walkable, worldPoint, x, y);
                        tile.SetColor(tile.neutralColor);
                        tile.name = worldPoint.ToString();
                        if (!tile.walkable)
                        {
                            tile.SetColor(Color.red);
                        }

                        grid[x, y] = tile;
                        tile.transform.position = worldPoint;
                    }
                    else
                    {
                        // Handle the case where there are not enough existing tiles
                        Debug.LogError("Not enough existing tiles to update the grid.");
                        return;
                    }
                }
            }
        }
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
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

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
