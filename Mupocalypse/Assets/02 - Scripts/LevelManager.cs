﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class LevelManager : Singleton<LevelManager>
{


    private int _roomID;
    private Animator fade;

    void Start()
    {
        fade = GetComponent<Animator>();
    }
    
    public void LoadScene()
    {
        SceneManager.LoadScene(_roomID + 1);
        
    }

    public void FadeIn()
    { 
        fade.SetTrigger("FadeIn");
    }

    public void FadeOut(int roomID)
    {
        this._roomID = roomID;
        fade.SetTrigger("FadeOut");
    }
    
    protected override void OnEnableCallback()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
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
