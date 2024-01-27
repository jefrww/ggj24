using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
enum NextCam 
{
    Body,
    Part
}

public class CameraController : MonoBehaviour
{
    public Camera mainCam;
    public Camera transitionCam;
    public List<Camera> partCams;
    public float transitionSpeed = .5f;

    public PlayerMovement mainBody;
    public List<PlayerMovement> partBodies;
    
    private int _currentStep = 0;
    private bool _isTransitioning = false;
    private float _completion = 0f;
    private NextCam next = NextCam.Part;

    private Vector3 _origin;

    private Vector3 _target;
    // Start is called before the first frame update
    void Awake()
    {
        mainCam.enabled = true;
        transitionCam.enabled = false;
        foreach (var cam in partCams)
        {
            cam.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isTransitioning && Input.GetKeyDown(KeyCode.K))
        {
            switch (next)
            {
                case NextCam.Part:
                    Debug.Log("transitioning to part");
                    goToNextPart();
                    break;
                case NextCam.Body:
                    Debug.Log("transitioning to robo");
                    goBackToRobot();
                    break;
            }
        }

        if (_isTransitioning)
        {
            if (_completion < 1) {
                _completion += Time.deltaTime * transitionSpeed;
                transitionCam.transform.position = Vector3.Slerp(_origin, _target, _completion);
            }
            else
            {
                _isTransitioning = false;
                _completion = 0f;
                transitionCam.enabled = false;
                switch (next)
                {
                    case NextCam.Body:
                        mainCam.enabled = true;
                        mainBody.isActive = true;
                        next = NextCam.Part;
                        _currentStep++;
                        break;
                    case NextCam.Part:
                        partCams[_currentStep].enabled = true;
                        partBodies[_currentStep].isActive = true;
                        next = NextCam.Body;
                        break;
                }
            }
        }
    }

    void goToNextPart()
    {
        mainBody.animator.SetBool("sendSignal", true);
        
        mainBody.isActive = false;
        mainCam.enabled = false;
        
        _origin = mainCam.transform.position;
        _target = partCams[_currentStep].transform.position;
        
        transitionCam.enabled = true;
        _isTransitioning = true;
    }

    void goBackToRobot()
    {
        partBodies[_currentStep].isActive = false;
        partCams[_currentStep].enabled = false;

        _origin = partCams[_currentStep].transform.position;
        _target = mainCam.transform.position;
        
        transitionCam.enabled = true;
        _isTransitioning = true;
    }
}
