using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance { get; private set; }
    public Transform seeker, target;
    public List<OverlayTile> tiles;
    GridManager grid;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one PathFinder! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        grid = FindObjectOfType<GridManager>();
        tiles = new List<OverlayTile>();
    }
    public void FindPath(Vector3 startPos, Vector3 targetPos, out int length)
    {
        OverlayTile startNode = TilesManager.Instance.GetTilesMap()[startPos];
        OverlayTile targetNode = TilesManager.Instance.GetTilesMap()[targetPos];

        Heap<OverlayTile> openSet = new Heap<OverlayTile>(grid.MaxSize);
        HashSet<OverlayTile> closedSet = new HashSet<OverlayTile>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            OverlayTile currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode, out int pathLength);
                length = pathLength;
                return;
            }

            foreach (OverlayTile neighbour in grid.GetNeighbours(currentNode))
            {

                if (neighbour == null || !neighbour.walkable || (neighbour.blocked && neighbour.currentUnit != startNode.currentUnit) || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        length = 0;
    }

    void RetracePath(OverlayTile startNode, OverlayTile endNode, out int length)
    {
        List<OverlayTile> path = new List<OverlayTile>();
        OverlayTile currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            tiles.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        length = path.Count;
        grid.path = path;

    }

    int GetDistance(OverlayTile nodeA, OverlayTile nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public void HighlightPath(List<OverlayTile> overlayTiles)
    {
        foreach (OverlayTile tile in overlayTiles)
        {
            tile.SetColor(Color.cyan);
        }
        StartCoroutine(ResetColor(0.5f, overlayTiles));
    }

    private IEnumerator ResetColor(float waitTime, List<OverlayTile> overlayTiles)
    {
        yield return new WaitForSeconds(waitTime);
        foreach (OverlayTile tile in overlayTiles)
        {
            tile.ResetColor();
        }
    }


}