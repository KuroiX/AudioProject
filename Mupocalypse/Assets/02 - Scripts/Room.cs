﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public SpawnPoint[] spawnPoint;
    
    public DespawnPoint[] despawnPoint;
    
    private SpawnPoint currentSpawnPoint;

    public Vector2 cameraMin;
    public Vector2 cameraMax;
    public CameraController cam;

    public bool savePointRoom = false;
    private GameObject[] doors;
    
    // Start is called before the first frame update
    void Start()
    {
        
        foreach (SpawnPoint sp in spawnPoint)
        {
            if (sp.spawnID == GameManager.Instance.spawnID)
                currentSpawnPoint = sp;
        }


        if(savePointRoom)
          GameManager.Instance.savePoint = currentSpawnPoint.spawnID;


        //if boss room
        if (ProgressManager.Instance.defeatedBosses.ContainsKey(SceneManager.GetActiveScene().buildIndex))
        {

            doors = GameObject.FindGameObjectsWithTag("Door");
            // if Boss hasn't been defeated
            if (!ProgressManager.Instance.defeatedBosses[SceneManager.GetActiveScene().buildIndex])
            {
                foreach (GameObject door in doors)
                {
                    door.GetComponent<Door>().Lock();
                }
            }
            else
            {
                foreach (GameObject door in doors)
                {
                    door.GetComponent<Door>().UnLock();
                }
            } 
            
            }
            
            

        SetPlayerPos();
        SetCameraPos();
        
        LevelManager.Instance.FadeIn();
    }
    

    public void DespawnCollectables()
    {
        
    }

    public void SetPlayerPos()
    {
        Player.Instance.transform.position = currentSpawnPoint.spawnPosition;
    }

    public void SetCameraPos()
    {

        Vector3 temp = Player.Instance.transform.Position2D().ToVec3();
        temp.z = -10;
        cam.transform.position = temp + new Vector3(0, 3.5f, 0); 
        cam.max = cameraMax;
        cam.min = cameraMin;

    }
    
}
