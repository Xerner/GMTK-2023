using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

[AddComponentMenu("UI/Slider Setting Bar")]
public class SliderSettingBar : UIInputBehaviour<float> {
    public GameSetting<float> gameSetting;
    [SerializeField] float upperBound = 0f;
    [SerializeField] float lowerBound = 100f;
    [SerializeField] float step = 5f;
    [SerializeField] Slider slider;
    bool navigateIsPressed = false;
    Action continuousUpdateFunc;
    [SerializeField] float waitTimeOnButtonHold = 1f;
    [SerializeField] float updateIntervalTime = 0.1f;
    float currentWaitTime = 1f;
     
    new void OnValidate() {
        //base.OnValidate();
        if (upperBound < lowerBound) upperBound = lowerBound;
        if (lowerBound > upperBound) lowerBound = upperBound;
    }

    protected override void Start() {
        if (!Application.isPlaying) return;
        if (slider == null) Debug.LogError("No slider provided");
        else
        {
            slider.onValueChanged.AddListener(UpdateValue);
            gameSetting.OnValueChange.AddListener(UpdateSlider);
        }
        base.Start();
    }

    void Update() {
        UpdateIteration();
    }

    void UpdateSlider(float value)
    {
        slider.value = value;
    }

    void UpdateIteration() {
        if (navigateIsPressed && continuousUpdateFunc != null) {
            currentWaitTime -= Time.deltaTime;
            if (currentWaitTime <= 0) {
                continuousUpdateFunc();
                currentWaitTime = updateIntervalTime;
            }
        }
    }

    public float ToUnit(float value) => value / step;
    
    public float FromUnit(float value) => value * step;

    public float GetUnitCount() => (upperBound - lowerBound) / step;

    public override GameSetting<float> GameSetting => gameSetting;

    public void UpdateValueUp() => UpdateValue(currentValue + step);
    
    public void UpdateValueDown() => UpdateValue(currentValue - step);

    public void UpdateValue(int siblingIndex) {
        float newValue = FromUnit(siblingIndex+1);
        UpdateValue(newValue);
    }

    public override void UpdateValue(float value) {
        float newValue = Mathf.Clamp(value, lowerBound, upperBound);
        base.UpdateValue(newValue);
    }

    public override void OnDeselect(BaseEventData data) {
        continuousUpdateFunc = null;
        currentWaitTime = waitTimeOnButtonHold;
        base.OnDeselect(data);
    }

    public override void UpdateValue(Vector2 navigationDirection)
    {
        if (navigationDirection.magnitude == 0) return;
        if (navigationDirection.x > 0f)
            continuousUpdateFunc = UpdateValueUp;
        else if (navigationDirection.x < 0f)
            continuousUpdateFunc = UpdateValueDown;
        else
            continuousUpdateFunc = null;
        if (continuousUpdateFunc != null)
            continuousUpdateFunc();
    }
}
