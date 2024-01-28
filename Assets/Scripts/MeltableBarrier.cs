
using System;
using UnityEngine;

public class MeltableBarrier : MonoBehaviour
{
    private void Update()
    {
        
    }

    public void MeltMe()
    {
        Debug.Log("MELTING DOWN A SQUARE");
        gameObject.SetActive(false);
    }
}

