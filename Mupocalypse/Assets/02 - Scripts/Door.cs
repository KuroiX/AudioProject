using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{

    private bool condition = false;

    public Animator disappear;

    public void Lock()
    {
        condition = false;
        GetComponent<Collider2D>().isTrigger = false;
    }
    
    public void UnLock()
    {
        
        disappear.SetTrigger("Unlock");
        condition = true;
        GetComponent<Collider2D>().isTrigger = true;
    }
    
    private void OnTriggerEnter2D(Collider2D player)
    {
        
        if (player.CompareTag("Player") && condition)
        {
            disappear.SetTrigger("Unlock");
        }
    }
}
