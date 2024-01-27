using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmitZone : MonoBehaviour
{
    // Start is called before the first frame update
    private BoxCollider2D triggerZone;
    public GameObject CameraTarget;

    public CameraController CameraController;
    
    void Awake()
    {
        triggerZone = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
