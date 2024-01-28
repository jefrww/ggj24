
using System;
using UnityEngine;

public class MeltableBarrier : MonoBehaviour
{
    private void Update()
    {
        
    }

    public void MeltMe()
    {
        GetComponentInChildren<Collider2D>().enabled = false;
        GetComponentInChildren<Animator>().SetBool("melt", true);
    }
}

