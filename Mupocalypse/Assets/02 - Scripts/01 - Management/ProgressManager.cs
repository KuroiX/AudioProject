using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : Singleton<ProgressManager>
{
    public Dictionary<int, bool> defeatedBosses = new Dictionary<int, bool>()
    {
        // roomId, bossIsDefeated 
        {0, false}
    };

    public List<Collectable> collectables = new List<Collectable>();

    public Dictionary<string, bool[]> activeCollectables = new Dictionary<string, bool[]>();

    protected override void OnEnableCallback() {
        SceneManager.sceneLoaded += OnLoad;
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnLoad(Scene scene, LoadSceneMode mode)
    {
        var objects = FindObjectsOfType<CollectableGO>();

        if (!activeCollectables.ContainsKey(scene.name))
        {
            activeCollectables[scene.name] = new bool[objects.Length];
        }

        for (int i = 0; i < objects.Length; i++)
        {
            if (activeCollectables[scene.name][i] == true)
                Destroy(objects[i].gameObject);
            objects[i].id = i;
        }
    }

    public void MarkDestroyed(int id)
    {
        activeCollectables[SceneManager.GetActiveScene().name][id] = true;
    }

    public void Reset()
    {
        collectables = new List<Collectable>();
        activeCollectables = new Dictionary<string, bool[]>();
        defeatedBosses = new Dictionary<int, bool>()
        {
            // roomId, bossIsDefeated 
            {0, false}
        };
    }
    

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            Destroy(gameObject);
        }
    }
}
