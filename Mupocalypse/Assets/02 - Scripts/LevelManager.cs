using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{


    private int _roomID;
    public Animator fade;
    public void LoadScene()
    {
        SceneManager.LoadScene(_roomID);
        
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

    
}
