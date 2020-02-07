using System;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Safe")]
    public AudioClip[] audioClips;
    public float masterVolume;
    public float effectVolume;
    public float musicVolume;
    public AudioMixer mixer;
    public AudioMixerSnapshot unpause;
    public AudioMixerSnapshot pause;

    void Start()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #region Menus & Sliders
    
    public void SetPaused(bool pause)
    {
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
    }

    void SetEffect(float value)
    {
        mixer.SetFloat("VolumeEffects", value);
    }

    void SetMusic(float value)
    {
        mixer.SetFloat("VolumeMusic", value);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Room room = GameObject.Find("Room").GetComponent<Room>();
        if (room.bossRoom)
        {
            //TODO if defeated
        }
    }

    public void PlayClip(AudioClip clip)
    {
        Debug.Log("play clip");
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    public void PlayCurrent()
    {
        GetComponent<AudioSource>().clip = audioClips[1];
        GetComponent<AudioSource>().Play();
    }

    public AudioClip GetCurrent()
    {
        return audioClips[1];
    }

    public AudioClip GetCurrentClip()
    {
        return GetComponent<AudioSource>().clip;
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
