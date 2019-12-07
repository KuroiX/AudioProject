using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{


    private int roomID;
    public Animator fade;
    public void LoadScene(int roomID)
    {
        FadeOut();
        this.roomID = roomID;
        SceneManager.LoadScene(roomID);
        
        FadeIn();
       
    }

    public void FadeIn()
    {
      fade.SetTrigger("FadeIn");
        
        
    }

    public void FadeOut()
    {
        fade.SetTrigger("FadeOut");
    }

    
}
