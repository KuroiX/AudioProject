using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSounds : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] bossSounds;

    void Start()
    {
        source = GetComponent<AudioSource>();
        
        int index = SceneManager.GetActiveScene().buildIndex;
        
        
        //if boss room
        if (index == 9)
        {
            Debug.Log(index);
            // if Boss hasn't been defeated
            if (!ProgressManager.Instance.defeatedBosses.ContainsKey(4) || !ProgressManager.Instance.defeatedBosses[4])
            {
                Debug.Log("Start");
                StartCoroutine(BossSoundsEnumerator());
            }
            else
            {
                Debug.Log("Defeated");
            }
        } 
        else if (index == 11)
        {
            // if Boss hasn't been defeated
            if (!ProgressManager.Instance.defeatedBosses.ContainsKey(12) || !ProgressManager.Instance.defeatedBosses[12])
            {
                StartCoroutine(BossSoundsEnumerator());
            }
            else
            {
                Debug.Log("Defeated");
            }
        } 
    }
    
    IEnumerator BossSoundsEnumerator()
    {
        while (true)
        {
            source.PlayOneShot(bossSounds[Random.Range(0, bossSounds.Length)]);
            yield return new WaitForSeconds(Random.Range(2, 5));
        }
    }
}
