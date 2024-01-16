using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class MoveAction : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveDir;
    Vector3 targetPosition;
    public float rotationSpeed = 180f;
    public float Velocity { get; private set; }
    [SerializeField] int speed;
    Animator anim;
    public List<OverlayTile> path;
    Unit unit;
    float mobility;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        path = new List<OverlayTile>();
        unit = GetComponent<Unit>();
        mobility = unit.mobility;
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
            Move(path[0].worldPosition);
        }

    }


    public void MovedOneTile(OverlayTile tile)
    {
        if (path.Contains(tile))
        {
            transform.LookAt(path[0].transform);
            path.RemoveAt(0);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + moveDir * speed * Time.deltaTime);
    }

    public void RotateTowardsTarget(Vector3 target)
    {

        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        float step = rotationSpeed * mobility * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
    }


    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
        RotateTowardsTarget(new Vector3(path[0].transform.position.x, transform.position.y, path[0].transform.position.z));
    }

    public void MoveToTile(OverlayTile tile)
    {
        if (unit.currentTile != null)
        {
            PathFinder.Instance.FindPath(unit, unit.currentTile.worldPosition, tile.worldPosition, out int pathlength);
            //path = GridManager.Instance.path;
            PathFinder.Instance.HighlightPath(path);

        }
        else
        {
            Move(FindClosestTileToUnit(unit).worldPosition);
        }

    }
    public OverlayTile FindClosestTileToUnit(Unit unit)
    {
        OverlayTile closestTile = null;
        float minDistance = float.MaxValue;

        foreach (OverlayTile tile in TilesManager.Instance.GetAllTiles())
        {
            if (!tile.walkable || tile.blocked)
                continue;

            float distance = Vector3.Distance(unit.transform.position, tile.worldPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }
}
