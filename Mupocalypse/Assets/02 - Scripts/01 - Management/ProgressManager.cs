using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressManager : Singleton<ProgressManager>
{
    public List<Collectable> collectables = new List<Collectable>();

    public Dictionary<Scene, bool[]> activeCollectables = new Dictionary<Scene, bool[]>();

    protected override void OnEnableCallback() {
        SceneManager.sceneLoaded += OnLoad;
    }

    private void OnLoad(Scene scene, LoadSceneMode mode)
    {
        var objects = FindObjectsOfType<CollectableGO>();

        if (!activeCollectables.ContainsKey(scene))
        {
            activeCollectables[scene] = new bool[objects.Length];
        }

        for (int i = 0; i < objects.Length; i++)
        {
            if (activeCollectables[scene][i] == true)
                Destroy(objects[i]);
            objects[i].id = i;
        }
    }

    public void MarkDestroyed(int id)
    {
        activeCollectables[SceneManager.GetActiveScene()][id] = true;
    }

    public void Reset()
    {
        collectables = new List<Collectable>();
        activeCollectables = new Dictionary<Scene, bool[]>();
    }
}
