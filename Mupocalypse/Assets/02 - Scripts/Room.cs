using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public SpawnPoint[] spawnPoint;
    
    public DespawnPoint[] despawnPoint;
    
    private SpawnPoint currentSpawnPoint;

    public Vector2 cameraMin;
    public Vector2 cameraMax;
    public CameraController cam;

    public bool savePointRoom = false;
    
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

        Vector3 temp = Player.Instance.transform.Position2D().ToVec3();
        temp.z = -10;
        cam.transform.position = temp; 
        cam.max = cameraMax;
        cam.min = cameraMin;

    }
    
}
