using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    private bool condition = false;

    public Animator disappear;

    public void Lock()
    {
        condition = false;
        GetComponent<Collider>().isTrigger = false;
    }
    
    public void UnLock()
    {
        condition = true;
        GetComponent<Collider>().isTrigger = true;
    }
    
    private void OnTriggerEnter2D(Collider2D player)
    {
        
        if (player.tag == "Player" && condition)
        {
            disappear.SetTrigger("Unlock");
        }
    }
}
