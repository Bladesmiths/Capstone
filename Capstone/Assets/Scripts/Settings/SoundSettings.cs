using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;


    // Start is called before the first frame update
    void Start()
    {
        LoadSoundPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMasterMixerVolume(float value)
    {
        RuntimeManager.GetVCA("vca:/Master").setVolume(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicMixerVolume(float value)
    {
        RuntimeManager.GetVCA("vca:/Music").setVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXMixerVolume(float value)
    {
        RuntimeManager.GetVCA("vca:/SFX").setVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void LoadSoundPrefs()
    {
        // Load master volume
        // 0 is default
        if (PlayerPrefs.GetFloat("MasterVolume") != 0)
        {
            RuntimeManager.GetVCA("vca:/Master").setVolume(PlayerPrefs.GetFloat("MasterVolume"));
            masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        }
        // Load music volume
        if (PlayerPrefs.GetFloat("MusicVolume") != 0)
        {
            RuntimeManager.GetVCA("vca:/Music").setVolume(PlayerPrefs.GetFloat("MusicVolume"));
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        // Load sfx volume
        if (PlayerPrefs.GetFloat("SFXVolume") != 0)
        {
            RuntimeManager.GetVCA("vca:/SFX").setVolume(PlayerPrefs.GetFloat("SFXVolume"));
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
    }
}
