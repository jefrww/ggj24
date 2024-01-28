using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Movement
{
    Horizontal,
    HorizontalBursts,
    Airtime,
    Jump,
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
    public float burstSpeed;

    public bool hasBody = false;
    public bool hasLegs = false;
    public bool hasHead = false;
    public bool hasHands = false;
    public bool hasJaw = false;
    public Animator animator;

    private Rigidbody2D _playerRigid;
    private Collider2D _playerBounds;

    public CircleCollider2D HeadMelterCol;

    private MeltableBarrier lastTouchedMelter;
    
    //private bool hasBody= true,hasLegs= true,hasHands= true,hasHead= true, hasJaw= true;
    //private bool hasBody= false,hasLegs= false,hasHands= false,hasHead= false, hasJaw= false;

    private bool _onGround = false, _onLeftWall = false, _onRightWall = false;


    void Awake()
    {
        _playerRigid = this.GetComponent<Rigidbody2D>();
        _playerBounds = this.GetComponent<Collider2D>();
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
        // get input and move player accordingly
        MovePlayer();
        
        checkMelters();
    }


    private void checkMelters()
    {
        if (hasHead && Input.GetKeyDown(KeyCode.E))
        {
            List<Collider2D> hitList = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = LayerMask.GetMask("Barrier");
            filter.useLayerMask = true;
            _playerRigid.OverlapCollider(filter, hitList);
            
            foreach (var col in hitList)
            {
                MeltableBarrier melt = col.transform.GetComponent<MeltableBarrier>();
                if (melt != null)
                {
                    melt.MeltMe();
                }
            }
        }
        
        
        
                
    }


    public void setLastTouchedBarrier(MeltableBarrier barrier)
    {
        lastTouchedMelter = barrier;
    }
    
    private void MovePlayer()
    {
        Vector3 velocity = Vector3.zero;
        Movement movement = Movement.None;

        if (hasOnlyBody())
        {
            // get input
            if (Input.GetKeyDown(KeyCode.A) && canGoLeft())
            {
                movement = Movement.HorizontalBursts;
                velocity += Vector3.left;
            }

            if (Input.GetKeyDown(KeyCode.D) && canGoRight())
            {
                movement = Movement.HorizontalBursts;
                velocity += Vector3.right;
            }
        }
        // use other functions for specifying unique movements
        else
        {
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
            if (Input.GetKeyDown(KeyCode.Space) && canJump())
            {
                movement = Movement.Jump;
                velocity = Vector3.up;
            }

            if (isInAir())
            {
                movement = Movement.Airtime;
            }
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
            case Movement.Jump:
                velocity *= jumpSpeed;
                break;
            case Movement.HorizontalBursts:
                velocity *= burstSpeed;
                break;
            case Movement.Airtime:
                velocity *= aircontrol;
                break;
            case Movement.None:
                break;
        }

        _playerRigid.AddForce(velocity);

        // reduce air control
        if (isInAir())
        {
            velocity *= aircontrol;
        }

        _playerRigid.AddForce(velocity);

        if (Input.GetKeyDown(KeyCode.Space) && canJump())
        {
            _playerRigid.AddForce(Vector2.up * jumpSpeed);
        }
    }

    private void FixedUpdate()
    {
        if (_playerRigid.velocity.sqrMagnitude > maxVelocity)
        {
            _playerRigid.velocity *= 0.75f;
        }
    }

    private void SetAnimation()
    {
        animator.SetFloat("horizontalVelocity", _playerRigid.velocity.x);
        animator.SetBool("pressedRight",Input.GetKeyDown(KeyCode.D));
        animator.SetBool("pressedLeft",Input.GetKeyDown(KeyCode.A));
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasBody = !hasBody;
            animator.SetBool("hasBody", hasBody);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            hasHands = !hasHands;
            animator.SetBool("hasHands", hasHands);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            hasHead = !hasHead;
            animator.SetBool("hasHead", hasHead);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasLegs = !hasLegs;
            animator.SetBool("hasLegs", hasLegs);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            hasJaw = !hasJaw;
            animator.SetBool("hasJaw", hasJaw);
        }
    }
    
    bool IsTouchingGround()
    {
        Vector2 playerPos = _playerRigid.transform.position;
        Vector2 extents = _playerBounds.bounds.extents;
        Vector2 playerLeft = new Vector2(playerPos.x - (extents.x * .98f), playerPos.y);
        Vector2 playerRight = new Vector2(playerPos.x + (extents.x * .98f), playerPos.y);
        
        return testGround(playerPos) || testGround(playerLeft) || testGround(playerRight);
    }

    bool testGround(Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, groundDistance,
            LayerMask.GetMask("Ground", "Wall"));
        
        if (hit.transform == null)
        {
            return false;
        }
        
        return hit.transform.gameObject.layer == _groundLayer;
    }
    (bool left, bool right) IsTouchingWalls()
    {
        Vector3 position = _playerRigid.transform.position;
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
        return hasBody || hasHands || hasHead || hasJaw || hasLegs;
    }

    bool canGoRight()
    {
        return hasBody || hasHands || hasHead || hasJaw || hasLegs;
    }

    bool canJump()
    {
        return hasLegs && (_onGround);
    }

    bool canWallClimb()
    {
        return hasHands && (_onRightWall || _onLeftWall);
    }

    bool canLaser()
    {
        return hasHead;
    }

    bool isInAir()
    {
        return !(_onRightWall || _onLeftWall || _onGround);
    }

    bool hasOnlyBody()
    {
     return hasBody && !(hasLegs || hasHead || hasHands ||hasJaw);
    }

    bool hasBodyAndLegs()
    {
        return hasBody && hasLegs &&!( hasHead || hasHands ||hasJaw);
    }

    bool hasBodyLegsAndArms()
    {
        return hasBody && hasLegs && hasHands  && !( hasHead ||hasJaw);
    }
}