using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BackgroundMusic;

    void Start()
    {
        GlobalSettings.MusicVolumeSetting.OnValueChange.AddListener(UpdateVolume);
        UpdateVolume(GlobalSettings.MusicVolumeSetting.Get());
    }

    public void UpdateVolume(float volume)
    {
        BackgroundMusic.volume = Mathf.Clamp(volume, 0f, 1f);
    }
}
