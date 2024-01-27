using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    public float moveSpeed;
    public float jumpSpeed;
    public float groundDistance;
    void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            velocity += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity += Vector3.right;
        }

        velocity = velocity.normalized * moveSpeed;
        rb.AddForce(velocity);
        
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Debug.Log(IsGrounded());
            rb.AddForce(Vector2.up * jumpSpeed);
        }
    }
    
    bool IsGrounded()
    {
        Debug.Log("RB: " + rb.transform.position);
        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position + Vector3.up, Vector3.down, groundDistance, LayerMask.GetMask("Ground"));
        Debug.Log("Hit: " +hit.point);
        if (hit.collider == null)
        {
            Debug.Log("NO HIT");
            return false;
        }
        return hit.collider.CompareTag("Ground");
    }
}
