using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource music;
    public AudioSource footstep1;
    public AudioSource footstep2;
    public AudioSource jump1;
    public AudioSource jump2;
    // Start is called before the first frame update
    void Awake()
    {
        music.loop = true;
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
