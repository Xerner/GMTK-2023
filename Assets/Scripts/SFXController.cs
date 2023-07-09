using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    public AudioSource audioSource;
    
    public AudioClip[] shootAudio;
    public AudioClip[] dashAudio;
    public AudioClip[] hitAudio;
    public AudioClip[] invadeAudio;

    public void PlayRandom(AudioClip[] clips) {
        if(clips.Length == 0){
            return;
        }
        int clipIndex = (int)(Random.Range(0, clips.Length));
        audioSource.PlayOneShot(clips[clipIndex]);
    }
}