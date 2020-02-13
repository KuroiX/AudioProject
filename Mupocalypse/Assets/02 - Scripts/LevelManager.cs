﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class LevelManager : Singleton<LevelManager>
{
    private int roomId;
    private Animator fade;

    void Awake()
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
        
    }

    /*public void OnEscape(InputAction.CallbackContext context)
    {
        Debug.Log("ah ja");
        if (context.started)
            SceneManager.LoadScene(0);
    }*/
    public void OnEscape()
    {
        SceneManager.LoadScene(0);
    }
}
