using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public int scene;
    public void StartGame()
    { 
        SceneManager.LoadScene(scene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Start() {
        Destroy(GameManager.Instance.gameObject);
        Destroy(LevelManager.Instance.gameObject);
        Destroy(ProgressManager.Instance.gameObject);
    }
}
