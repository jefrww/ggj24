using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCam;
    // public List<Camera> partCams;
    public float transitionSpeed = .5f;

    public PlayerMovement mainBody;
    public List<PlayerMovement> partBodies;
    
    private int _currentStep = 0;
    private bool _isTransitioning = false;
    private float _completion = 0f;

    private Vector3 _origin;

    private Vector3 _target;
    private PlayerMovement _targetPart;
    
    // Start is called before the first frame update
    void Awake()
    {
        mainCam.enabled = true;
        _targetPart = mainBody;

        // foreach (var cam in partCams)
        // {
        //     cam.enabled = false;
        // }
    }

    public void setTargetPart(PlayerMovement newTarget)
    {
        if (newTarget != mainBody)
        {
            _currentStep++;
        }
        _target = newTarget.transform.position;
        _target.z = this.transform.position.z;
        _origin = _targetPart.transform.position;
        _origin.z = this.transform.position.z;
        _targetPart = newTarget;

        
        _isTransitioning = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!_isTransitioning && Input.GetKeyDown(KeyCode.K))
        {
            if (_targetPart == mainBody)
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
            if (_completion < 1) {
                _completion += Time.deltaTime * transitionSpeed;
                mainCam.transform.position = Vector3.Slerp(_origin, _target, _completion);
            }
            else
            {
                _isTransitioning = false;
                _completion = 0f;
            }
        }
        else
        {
            mainCam.transform.position = new Vector3(_targetPart.transform.position.x, _targetPart.transform.position.y, this.transform.position.z);
        }
    }

    void goBackToRobot()
    {
        
        _isTransitioning = true;
    }
}
