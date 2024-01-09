using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class MoveAction : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveDir;
    Vector3 targetPosition;
    [SerializeField] int speed;
    Animator anim;
    public List<OverlayTile> path;
    Unit unit;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        path = new List<OverlayTile>();
        unit = GetComponent<Unit>();
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

        float velocity = Mathf.Max(Mathf.Abs(moveDir.x), Mathf.Abs(moveDir.z));
        anim.SetFloat("Velocity", velocity);
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


    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public void MoveToTile(OverlayTile tile)
    {
        PathFinder.Instance.FindPath(unit.currentTile.worldPosition, tile.worldPosition, out int pathlength);
        path = GridManager.Instance.path;
        PathFinder.Instance.HighlightPath(path);
    }

}
