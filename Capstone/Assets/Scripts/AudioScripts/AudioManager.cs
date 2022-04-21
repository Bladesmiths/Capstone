using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "BalancingData", menuName = "ScriptableObjects/AudioManager")]
public class AudioManager : SerializedScriptableObject
{
    public Dictionary<string, EventReference> soundEffects = new Dictionary<string, EventReference>();


    public void PlaySound(string name)
    {
        if(soundEffects.ContainsKey(name))
        {
            RuntimeManager.PlayOneShot(soundEffects[name]);
        }
    }

    public void Play3DSound(string name, Vector3 position)
    {
        if (soundEffects.ContainsKey(name))
        {
            RuntimeManager.PlayOneShot(soundEffects[name], position);
        }
    }

}
