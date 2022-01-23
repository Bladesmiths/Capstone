using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class SoundSettings : MonoBehaviour
{
    private float masterVolume = 1f;
    private float soundEffectsVolume = 1f;
    private float musicVolume = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMasterMixerVolume(float value)
    {
        masterVolume = value;
        float dbVolume = (value * 100f) - 100f;
        //RuntimeManager.GetVCA("vca:/Master").setVolume(value);
    }
    
    private void LoadSoundPrefs()
    {

    }
}
