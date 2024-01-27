using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float jumpSpeed;
    public float groundDistance, wallDistance;
    
    private Rigidbody2D rb;
    private bool hasTorso= false,hasLegs= false,hasArms= false,hasHead= false, hasJaw= false;
    private bool _onGround= false,_onLeftWall= false, _onRightWall = false;


    void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    void Update()
    {   
        // get current state of the player first
        _onGround = IsTouchingGround();
        (_onLeftWall, _onRightWall) = IsTouchingWalls();
        Vector3 velocity = Vector3.zero;
        // get input
        if (Input.GetKey(KeyCode.A) && canGoLeft())
        {
            velocity += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D) && canGoRight())
        {
            velocity += Vector3.right;
        }
        if (Input.GetKey(KeyCode.W) && canWallClimb())
        {
            velocity += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S) && canWallClimb())
        {
            velocity += Vector3.down;
        }

        velocity = velocity.normalized * moveSpeed;
        rb.AddForce(velocity);
        
        if (Input.GetKeyDown(KeyCode.Space) && canJump())
        {
            rb.AddForce(Vector2.up * jumpSpeed);
        }
    }
    
    bool IsTouchingGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, Vector3.down, groundDistance, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            return false;
        }
        return hit.collider.CompareTag("Ground");
    }
    
    (bool left, bool right) IsTouchingWalls()
    {
        Vector3 position = rb.transform.position;
        bool touchingLeft = false, touchingRight = false;
        RaycastHit2D leftHit = Physics2D.Raycast(position, Vector3.left, wallDistance, LayerMask.GetMask("Wall"));
        RaycastHit2D rightHit = Physics2D.Raycast(position, Vector3.right, wallDistance, LayerMask.GetMask("Wall"));

        if (leftHit.collider != null && leftHit.collider.CompareTag("Wall"))
        {
            touchingLeft = true;
        }
        
        if (rightHit.collider != null && rightHit.collider.CompareTag("Wall"))
        {
            touchingRight = true;
        }

        return (touchingLeft, touchingRight);
    }

    bool canGoLeft()
    {
        return hasTorso && hasArms && hasHead && hasJaw;
    }

    bool canGoRight()
    {
        return hasTorso && hasArms && hasHead && hasJaw;
    }

    bool canJump()
    {
        return hasLegs;
    }

    bool canWallClimb()
    {
        return hasArms;
    }

    bool canLaser()
    {
        return  hasHead ;
    }
    
}
