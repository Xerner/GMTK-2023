using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialAudioController : MonoBehaviour
{
    public AudioSource introSource;
    public AudioSource loopSource;

    void Awake() {
        playAudioSequentially();
    }

    void playAudioSequentially() {
        introSource.Play();
        loopSource.PlayDelayed(introSource.clip.length);
    }
}
