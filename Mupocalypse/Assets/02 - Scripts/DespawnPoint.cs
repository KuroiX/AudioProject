using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnPoint : MonoBehaviour
{
    public int nextSpawnID;
    private void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag == "Player")
        {
            setNextSpawnID();
            
        }
    }

    public void setNextSpawnID()
    {
        GameManager.Instance.spawnID = nextSpawnID;
    }
    

}
