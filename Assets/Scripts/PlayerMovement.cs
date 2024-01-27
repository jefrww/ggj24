using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Movement 
{
    Horizontal,
    Vertical,
    None
}

public class PlayerMovement : MonoBehaviour
{
    private int _groundLayer = 6;
    private int _magnetLayer = 7;
    public float horizontalSpeed, verticalSpeed;
    public float jumpSpeed;
    public float groundDistance, wallDistance;
    public float maxVelocity;
    public float aircontrol;
    public bool isActive;

    public Animator animator;
    
    private Rigidbody2D rb;
    //private bool hasBody= true,hasLegs= true,hasHands= true,hasHead= true, hasJaw= true;
    //private bool hasBody= false,hasLegs= false,hasHands= false,hasHead= false, hasJaw= false;
    private Dictionary<string, bool> bodyConfig = new Dictionary<string, bool>()
    {
        {"hasBody", true},
        {"hasHands", false},
        {"hasHead", false},
        {"hasLegs", false},
        {"hasJaw", false},
    };

    private bool _onGround= false,_onLeftWall= false, _onRightWall = false;


    void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        _groundLayer = LayerMask.GetMask("Ground");
        _magnetLayer = LayerMask.GetMask("Wall");
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        // update the animation
        SetAnimation();
        // get current state of the player first
        _onGround = IsTouchingGround();
        (_onLeftWall, _onRightWall) = IsTouchingWalls();
        Debug.Log("Ground: " +_onGround);
        Debug.Log("Left Wall: " +_onLeftWall);
        Debug.Log("Right Wall: "+_onRightWall);

        Vector3 velocity = Vector3.zero;
        Movement movement = Movement.None;
        // get input
        if (Input.GetKey(KeyCode.A) && canGoLeft())
        {
            movement = Movement.Horizontal;
            velocity += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D) && canGoRight())
        {
            movement = Movement.Horizontal;
            velocity += Vector3.right;
        }
        if (Input.GetKey(KeyCode.W) && canWallClimb())
        {
            movement = Movement.Vertical;
            velocity += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S) && canWallClimb())
        {
            movement = Movement.Vertical;
            velocity += Vector3.down;
        }
        // normalize change in speed to ensure consistency
        velocity = velocity.normalized;
        
        
        
        // apply movement
        switch (movement)
        {
            case Movement.Vertical:
                velocity *= verticalSpeed;
                break;
            case Movement.Horizontal:
                velocity *= horizontalSpeed;
                break;
            case Movement.None:
                break;
        }
        
        // reduce air control
        if (isInAir())
        {
            velocity *= aircontrol;
        }

        rb.AddForce(velocity);
        
        if (Input.GetKeyDown(KeyCode.Space) && canJump())
        {
            rb.AddForce(Vector2.up * jumpSpeed);
        }
    }

    private void FixedUpdate()
    {
        if( rb.velocity.sqrMagnitude > maxVelocity )
        {
            rb.velocity *= 0.75f;
        }
    }

    private void SetAnimation()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            bodyConfig["hasBody"] = !bodyConfig["hasBody"];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            bodyConfig["hasHands"] = !bodyConfig["hasHands"];
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            bodyConfig["hasHead"] = !bodyConfig["hasHead"];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            bodyConfig["hasLegs"] = !bodyConfig["hasLegs"];
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            bodyConfig["hasJaw"] = !bodyConfig["hasJaw"];
        }

        foreach (var entry in bodyConfig)
        {
            animator.SetBool(entry.Key, entry.Value);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            foreach (var entry in bodyConfig)
            {
                bodyConfig[entry.Key] = false;
                animator.SetBool(entry.Key, entry.Value);
            }
        }
    }

    bool IsTouchingGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, Vector3.down, groundDistance, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            return false;
        }
        
        return hit.collider.gameObject.layer == _groundLayer;
    }
    
    (bool left, bool right) IsTouchingWalls()
    {
        Vector3 position = rb.transform.position;
        bool touchingLeft = false, touchingRight = false;
        RaycastHit2D leftHit = Physics2D.Raycast(position, Vector3.left, wallDistance, LayerMask.GetMask("Wall"));
        RaycastHit2D rightHit = Physics2D.Raycast(position, Vector3.right, wallDistance, LayerMask.GetMask("Wall"));

        if (leftHit.collider != null && leftHit.collider.gameObject.layer == _magnetLayer)
        {
            touchingLeft = true;
        }
        
        if (rightHit.collider != null && rightHit.collider.gameObject.layer == _magnetLayer)
        {
            touchingRight = true;
        }

        return (touchingLeft, touchingRight);
    }

    bool canGoLeft()
    {
        return bodyConfig["hasBody"] || bodyConfig["hasHands"] || bodyConfig["hasHead"] || bodyConfig["hasJaw"];
    }

    bool canGoRight()
    {
        return bodyConfig["hasBody"] || bodyConfig["hasHands"] || bodyConfig["hasHead"] || bodyConfig["hasJaw"];
    }

    bool canJump()
    {
        return bodyConfig["hasLegs"] && (_onGround);
    }

    bool canWallClimb()
    {
        return bodyConfig["hasHands"] && (_onRightWall || _onLeftWall);
    }

    bool canLaser()
    {
        return  bodyConfig["hasHead"] ;
    }
    bool isInAir()
    {
        return !(_onRightWall || _onLeftWall || _onGround) ;
    }
    
}
