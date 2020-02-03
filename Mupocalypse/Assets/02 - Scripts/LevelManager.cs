using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class LevelManager : Singleton<LevelManager>
{


    private int roomId;
    private Animator fade;

    void Start()
    {
        fade = GetComponent<Animator>();
    }
    
    public void LoadScene()
    {
        SceneManager.LoadScene(roomId + 1);
        
    }

    public void FadeIn()
    { 
        fade.SetTrigger("FadeIn");
    }

    public void FadeOut(int roomID)
    {
        this.roomId = roomID;
        fade.SetTrigger("FadeOut");
    }
    
    protected override void OnEnableCallback()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Player.Instance.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        if (scene.buildIndex == 0)
        {
            Destroy(gameObject);
        }
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.started)
            SceneManager.LoadScene(0);
    }
}
