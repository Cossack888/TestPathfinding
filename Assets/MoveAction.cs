using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveAction : MonoBehaviour
{
    Rigidbody rb;
    Vector3 moveDir;
    public Vector3 targetPosition;
    [SerializeField] int speed;
    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
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
    }


    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + moveDir * speed * Time.deltaTime);
    }


    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }


}
