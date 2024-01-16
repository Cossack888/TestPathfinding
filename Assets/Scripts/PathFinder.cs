using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathFinder : MonoBehaviour
{
    public static PathFinder Instance { get; private set; }
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
    }
    public void FindPath(Unit unit, Vector3 startPos, Vector3 targetPos, out List<OverlayTile> pathNodesAll)
    {
        OverlayTile startNode = unit.CurrentTile;
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
                RetracePath(unit, startNode, targetNode, out List<OverlayTile> pathNodes);
                pathNodesAll = pathNodes;
                return;
            }

            foreach (OverlayTile neighbour in grid.GetNeighbours(currentNode))
            {

                if (neighbour == null || !neighbour.Walkable || (neighbour.Blocked && neighbour.CurrentUnit != startNode.CurrentUnit) || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    neighbour.SetCosts(newMovementCostToNeighbour, GetDistance(neighbour, targetNode), currentNode);

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        pathNodesAll = null;
    }

    void RetracePath(Unit unit, OverlayTile startNode, OverlayTile endNode, out List<OverlayTile> pathNodes)
    {
        List<OverlayTile> path = new List<OverlayTile>();
        OverlayTile currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        pathNodes = path;

    }



    int GetDistance(OverlayTile nodeA, OverlayTile nodeB)
    {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

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