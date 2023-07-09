using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mixer;
    private float maxVolume;

    void Start()
    {
        mixer.GetFloat("MasterVolume", out maxVolume);
        GlobalSettings.MusicVolumeSetting.OnValueChange.AddListener(UpdateVolume);
        UpdateVolume(GlobalSettings.MusicVolumeSetting.Get());
    }

    public void UpdateVolume(float volume)
    {
         mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
}
