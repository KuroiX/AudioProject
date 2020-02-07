using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioDelegate : MonoBehaviour
{
    public Slider masterSlider;
    public Slider effectSlider;
    public Slider musicSlider;

    private AudioManager am;
    void Start()
    {
        am = AudioManager.Instance;
        SetSlider();
    }
    
    public void OnMasterChanged(float value)
    {
        am.OnMasterChanged(value);
    }

    public void OnEffectChanged(float value)
    {
        am.OnEffectChanged(value);
    }

    public void OnMusicChanged(float value)
    {
        am.OnMusicChanged(value);        
    }

    public void SetSlider()
    {
        masterSlider.value = am.masterVolume;
        effectSlider.value = am.effectVolume;
        musicSlider.value = am.musicVolume;
    }
}
