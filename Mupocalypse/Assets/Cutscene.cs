using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Next");
    }

    IEnumerator Next()
    {
        yield return new WaitForSeconds(28);
        GameManager.Instance.GetComponent<AudioSource>().mute = false;
        SceneManager.LoadScene(1);
    }
}
