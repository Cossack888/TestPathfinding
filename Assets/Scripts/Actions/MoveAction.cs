using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveDir;
    Vector3 targetPosition;
    [SerializeField] float rotationSpeed = 180f;
    public float Velocity { get; private set; }
    int speed;
    Animator anim;
    List<OverlayTile> path;
    Unit unit;
    float mobility;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        path = new List<OverlayTile>();
        unit = GetComponent<Unit>();
        mobility = Random.Range(1, 5);
        speed = Random.Range(5, 10);
        StartCoroutine(ContinuousMovementCheck());

    }
    public void ClearPath()
    {
        path.Clear();
    }
    private void LateUpdate()
    {
        if (targetPosition != Vector3.zero)
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                moveDir = Vector3.zero;
            }
            else
            {
                moveDir = (targetPosition - transform.position).normalized;
            }
        }
        else
        {
            moveDir = Vector3.zero;
        }

        Velocity = Mathf.Max(Mathf.Abs(moveDir.x), Mathf.Abs(moveDir.z));
        anim.SetFloat("Velocity", Velocity);

        if (path.Count > 0)
        {
            Move(path[0].WorldPosition);
            RotateTowardsTarget(targetPosition);
        }

    }
    private IEnumerator ContinuousMovementCheck()
    {
        while (true)
        {
            yield return null;

            if (path.Count > 0 && Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                MovedOneTile(path[0]);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + moveDir * speed * Time.deltaTime);
    }


    public void RotateTowardsTarget(Vector3 target)
    {
        if (Velocity < 0.1f)
        {
            return;
        }

        Vector3 direction = target - transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        float step = rotationSpeed * mobility * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }


    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;

    }

    public void MovedOneTile(OverlayTile tile)
    {
        if (path.Contains(tile))
        {
            path.RemoveAt(0);

            if (path.Count == 0)
            {
                targetPosition = Vector3.zero;
                anim.SetFloat("Velocity", 0f);
            }
        }
    }

    public void MoveToTile(OverlayTile tile)
    {
        if (unit.CurrentTile != null)
        {
            PathFinder.Instance.FindPath(unit, unit.CurrentTile.WorldPosition, tile.WorldPosition, out List<OverlayTile> pathNodes);

            if (pathNodes != null && pathNodes.Count > 0)
            {
                path = pathNodes;
                PathFinder.Instance.HighlightPath(path);
                return;
            }
        }

        Move(FindClosestTileToUnit(unit).WorldPosition);
    }

    public OverlayTile FindClosestTileToUnit(Unit unit)
    {
        OverlayTile closestTile = null;
        float minDistance = float.MaxValue;

        foreach (OverlayTile tile in TilesManager.Instance.GetAllTiles())
        {
            if (!tile.Walkable || tile.Blocked)
                continue;

            float distance = Vector3.Distance(unit.transform.position, tile.WorldPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }
}
