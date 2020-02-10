using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Safe")]
    public AudioClip[] audioClips;
    public AudioClip current;
    public float masterVolume;
    public float effectVolume;
    public float musicVolume;
    public AudioMixer mixer;
    public AudioMixerSnapshot unpause;
    public AudioMixerSnapshot pause;

    public AudioSource[] sources;

    #region MonoBehavior
    
    void Start()
    {
        current = audioClips[1];
        sources = GetComponents<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sources[0].mute = false;
        
        if (scene.buildIndex == SceneManager.sceneCountInBuildSettings - 1)
        {
            //Debug.Log("Here we play nothing because the intro plays");
            sources[0].mute = true;
        }
        else if (scene.buildIndex == 0)
        {
            //Debug.Log("Here we play the menu music");
            if (sources[0].clip != audioClips[0])
            {
                StartCoroutine(FadeTo(0.2f, audioClips[0]));
            }
        }
        else
        {
            //Debug.Log("We are in a different room: " + scene.buildIndex);
            Room room = GameObject.Find("Room").GetComponent<Room>();
            if (room.bossRoom)
            {
                bool bossDefeated =
                    ProgressManager.Instance.defeatedBosses.ContainsKey(SceneManager.GetActiveScene().buildIndex)
                    && ProgressManager.Instance.defeatedBosses[SceneManager.GetActiveScene().buildIndex];
                if (!bossDefeated)
                {
                    //Debug.Log("We play the boss music (of the right boss)");
                    //TODO: use right boss fight music
                    StartCoroutine(FadeTo(0.4f, audioClips[3]));
                }
                else
                {
                    //Debug.Log("We play normal music");
                    if (sources[0].clip != current)
                    {
                        StartCoroutine(FadeTo(0.4f, current));
                    }
                }
            }
            else
            {
                //Debug.Log("We play normal music2");
                if (sources[0].clip != current)
                {
                    StartCoroutine(FadeTo(0.4f, current));
                }
            }
        }
    }
    
    #endregion

    #region Menus & Sliders
    
    public void SetPaused(bool pause)
    {
        //Debug.Log("SetPaused: " + pause);
        if (pause)
        {
            this.pause.TransitionTo(.01f);
        }
        else
        {
            unpause.TransitionTo(.01f);
        }
    }

    public void OnMasterChanged(float value)
    {
        masterVolume = value;
        SetMaster(value);
    }

    public void OnEffectChanged(float value)
    {
        effectVolume = value;
        SetEffect(value);
    }

    public void OnMusicChanged(float value)
    {
        musicVolume = value;
        SetMusic(value);
    }
    
    #endregion

    void SetMaster(float value)
    {
        mixer.SetFloat("VolumeMaster", value);
        masterVolume = value;
    }

    void SetEffect(float value)
    {
        mixer.SetFloat("VolumeEffects", value);
        effectVolume = value;
    }

    void SetMusic(float value)
    {
        mixer.SetFloat("VolumeMusic", value);
        musicVolume = value;
    }

    void PlayClip(AudioClip clip)
    {
        sources[0].clip = clip;
        sources[0].Play();
    }

    public void StartFade(float duration, AudioClip clip)
    {
        StartCoroutine(FadeTo(duration, clip));
    }

    IEnumerator FadeTo(float duration, AudioClip clip)
    {
        //Debug.Log("FadeTo");
        float currentTime = 0;
        float currentVolume = masterVolume;

        while (currentTime < duration)
        {
            SetMaster(currentVolume-(currentTime/duration * (80f-currentVolume)));
            currentTime += Time.deltaTime;
            yield return null;
        }
        
        SetMaster(currentVolume);
        PlayClip(clip);
    }
    

    public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }
}
