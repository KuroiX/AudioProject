using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DespawnPoint : MonoBehaviour
{
    public int nextSpawnID;
    public int nextRoom;
    private void OnTriggerEnter2D(Collider2D player)
    {
        
        if (player.tag == "Player")
        {
            setNextSpawnID();
            LevelManager.Instance.FadeOut(nextRoom);
        }
    }

    public void setNextSpawnID()
    {
        GameManager.Instance.spawnID = nextSpawnID;
    }
    

}
