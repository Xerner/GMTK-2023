using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float BobbingDistance = 10f;
    public float BobbingCycleTime = 1f;
    Vector3 startingPosition;
    
    void Start()
    {
        startingPosition = transform.position;
        StartAnimation();
    }

    public void StartAnimation()
    {
        LTDescr descr = LeanTween.move(gameObject, startingPosition + new Vector3(0f, BobbingDistance, 0f), BobbingCycleTime / 2);
        descr.setLoopPingPong();
        descr.setEaseInOutSine();
    }

    public void StopAnimation()
    {
        LeanTween.cancel(gameObject);
    }
}
