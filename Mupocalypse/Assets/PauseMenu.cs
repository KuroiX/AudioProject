﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool open;

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private GameObject abilites;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!open)
            {
                OpenMenu();
            }
            else
            {
                CloseMenu();
            }
        }
    }


    public void OpenMenu()
    {
        AudioManager.Instance.SetPaused(true);
        Time.timeScale = 0;
        open = true;
        Player.Instance.paused = true;
        panel.SetActive(true);
    }

    public void CloseMenu()
    {
        Time.timeScale = 1;
        open = false;
        Player.Instance.paused = false;
        Abilites(false);
        Volume(false);
        AudioManager.Instance.SetPaused(false);
        panel.SetActive(false);
    }

    public void Abilites(bool showAbilities)
    {
        foreach (GameObject gO in buttons)
        {
            gO.SetActive(!showAbilities);
        }
        
        abilites.SetActive(showAbilities);
    }

    public GameObject volumePanel;

    public void Volume(bool showVolume)
    {
        //TODO: ??
        //AudioManager.Instance.SetPaused(showVolume);
        
        foreach (GameObject gO in buttons)
        {
            gO.SetActive(!showVolume);
        }
        
        volumePanel.SetActive(showVolume);
    }

    public void Exit()
    {
        AudioManager.Instance.SetPaused(false);
        LevelManager.Instance.OnEscape(); 
    }
}
