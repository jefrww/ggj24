using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCam;

    // public List<Camera> partCams;
    public float transitionSpeed = .5f;

    public PlayerMovement mainBody;
    public List<PlayerMovement> partBodies;
    public List<TransmitZone> transmitZones;

    private int _currentStep = 0;
    private bool _isTransitioning = false;
    private float _completion = 0f;

    private Vector3 _originPosition;
    private Vector3 _targetPosition;
    private PlayerMovement _targetPlayer;

    private Rigidbody2D mainRB;

    // Start is called before the first frame update
    void Awake()
    {
        mainCam.enabled = true;
        activateTargetPlayer(mainBody);
        _targetPlayer = mainBody;
        mainRB = mainBody.GetComponent<Rigidbody2D>();
    }

    public void setTargetPart(PlayerMovement newTarget)
    {
        if (!isMainBodyTarget())
        {
            // Advance the artifical counter if we are NOT returning to the body
            _currentStep++;
        }
        mainBody.animator.SetBool("sendSignal", isMainBodyTarget());

        _targetPosition = newTarget.transform.position;
        _targetPosition.z = this.transform.position.z;
        _originPosition = _targetPlayer.transform.position;
        _originPosition.z = this.transform.position.z;
        _targetPlayer = newTarget;
        
        _isTransitioning = true;
    }

    // Update is called once per frame
    void Update()
    {
        // reset the animation to merge body parts once it's done
        // mainBody.animator.SetBool("combineParts", false);
        
        TransmitZone inZone = isPlayerInTransmitZone();
        if (!_isTransitioning && Input.GetKeyDown(KeyCode.K))
        {
            if (_targetPlayer == mainBody)
            {
                setTargetPart(partBodies[_currentStep]);
            }
            else
            {
                setTargetPart(mainBody);
            }
        }

        if (_isTransitioning)
        {
            if (_completion < 1)
            {
                _completion += Time.deltaTime * transitionSpeed;
                mainCam.transform.position = Vector3.Slerp(_originPosition, _targetPosition, _completion);
            }
            else
            {
                // Finished anuimating camera
                _isTransitioning = false;
                activateTargetPlayer(_targetPlayer);
                mainBody.animator.SetBool("sendSignal", !isMainBodyTarget());
                _completion = 0f;
            }
        }
        else
        {
            mainCam.transform.position = new Vector3(_targetPlayer.transform.position.x,
                _targetPlayer.transform.position.y, this.transform.position.z);
        }
        
        
        
        mainBody.animator.SetBool("inTransmissionZone", inZone != null);
        
        if (!_isTransitioning)
        {
            if (isMainBodyTarget() && inZone && Input.GetKeyDown(KeyCode.E)) {
                setTargetPart(inZone.targetPlayer);
            }
            else if (!isMainBodyTarget() && inZone && inZone.targetPlayer == _targetPlayer)
            {
                Debug.Log("REUNITED!");
                mainBody.animator.SetBool("sendSignal", false);
                mainBody.animator.SetBool("combineParts", true);
                mainBody.animator.SetBool("hasBody", true);
                if (_targetPlayer.hasLegs)
                {
                    mainBody.hasLegs = true;
                    mainBody.animator.SetBool("hasLegs", true);
                }
                if (_targetPlayer.hasHands)
                {
                    mainBody.hasHands = true;
                    mainBody.animator.SetBool("hasHands", true);
                }
                if (_targetPlayer.hasHead)
                {
                    mainBody.hasHead = true;
                    mainBody.animator.SetBool("hasHead", true);
                }
                if (_targetPlayer.hasJaw)
                {
                    mainBody.hasJaw = true;
                    mainBody.animator.SetBool("hasJaw", true);
                }
                inZone.gameObject.SetActive(false);
                GameObject toDeactivate = _targetPlayer.transform.parent.gameObject;

                setTargetPart(mainBody);
                toDeactivate.SetActive(false);
            }            
        }

        if (mainBody.hasJaw && Input.GetKeyDown(KeyCode.E))
        {
            mainBody.animator.SetBool("startLaughing", true);
        }

    }

    private void haltPlayer(PlayerMovement player)
    {
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    private TransmitZone isPlayerInTransmitZone()
    {
        RaycastHit2D hit = Physics2D.Raycast(_targetPlayer.transform.position, Vector3.down, 0,
            LayerMask.GetMask("Transmit"));
        if (hit.collider != null)
        {
            TransmitZone foundZone = hit.collider.GetComponent<TransmitZone>();
            return foundZone;
        }

        return null;
    }

    private void activateTargetPlayer(PlayerMovement target)
    {
        mainBody.isActive = target == mainBody;
        haltPlayer(mainBody);
        foreach (var part in partBodies)
        {
            haltPlayer(part);
            part.isActive = part == target;
        }
    }

    private bool isMainBodyTarget()
    {
        return _targetPlayer == mainBody;
    }
}