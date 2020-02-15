using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public int scene;
    public void StartGame()
    { 
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings-1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Start() {
        // Destroy(GameManager.Instance.gameObject);
        // Destroy(LevelManager.Instance.gameObject);
        // Destroy(ProgressManager.Instance.gameObject);
        ProgressManager.Instance?.Reset();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.spawnID = -1;
            GameManager.Instance.GetComponent<AudioSource>().mute = true;
        }
        // if (Player.Instance != null)
        //     Destroy(Player.Instance.gameObject);
        // if (LevelManager.Instance != null)
        //     Destroy(LevelManager.Instance.gameObject);
    }


    public GameObject[] buttons;
    public GameObject options;
    public void Options(bool on)
    {
        foreach (GameObject gO in buttons)
        {
            gO.SetActive(!on);
        }

        options.SetActive(on);
    }
}
