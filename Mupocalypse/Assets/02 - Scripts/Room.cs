﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public SpawnPoint[] spawnPoint;
    
    public DespawnPoint[] despawnPoint;

    private SpawnPoint currentSpawnPoint;
    
    public Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (SpawnPoint sp in spawnPoint)
        {
            if (sp.spawnID == GameManager.Instance.spawnID)
                currentSpawnPoint = sp;
        }
        SetPlayerPos();
        SetCameraPos();
        LevelManager.Instance.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        /*Camera.cameraMaxX  = currentSpawnPoint.cameraMaxPosX;
        Camera.cameraMaxY = currentSpawnPoint.CameraMaxPosY;*/
    }
    
}
