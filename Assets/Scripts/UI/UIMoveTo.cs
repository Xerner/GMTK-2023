using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

[AddComponentMenu("UI/Move To")]
[RequireComponent(typeof(RectTransform))]
public class UIMoveTo : MonoBehaviour
{
    [SerializeField] MoveToTarget target;
    [SerializeField] bool RunInUpdateLoop = false;
    bool hasMoved = false;

    LTDescr currentTween;

    public static Vector3 DefaultPosition = Vector3.zero;

    void Start()
    {
        if (target.GameObject != null) Move();
        if (!RunInUpdateLoop) enabled = false;
    }

    void Update() {
        if (target.CurrentMoveToDuration == 0f) {
            transform.position = GetPosition(target.GameObject);
            return;
        }
        if (!LeanTween.isTweening(gameObject) && target != null) Move();
    }

    public void Move(Vector3 targetPosition) {
        if (LeanTween.isTweening(gameObject)) LeanTween.cancel(gameObject);
        if (Application.isPlaying)
        {
            currentTween = LeanTween.move((RectTransform)transform, targetPosition, target.CurrentMoveToDuration);
            if (!hasMoved) currentTween.setOnComplete(() => {
                hasMoved = true;
                target.CurrentMoveToDuration = target.MoveToDuration;
            });
        }
        else
        {
            Debug.Log($"Moving to {targetPosition}");
            Vector3 position = GetPosition(target.GameObject);
            RectTransform rectTransform = (RectTransform)transform;
            switch (target.TargetPositionType)
            {
                case EPosition.Global:
                    rectTransform.position = position;
                    break;
                case EPosition.Local:
                    rectTransform.localPosition = position;
                    break;
                case EPosition.Anchored:
                    rectTransform.anchoredPosition = position;
                    break;
                default:
                    rectTransform.position = position;
                    break;
            }
        }
    }

    public Vector3 GetPosition(GameObject gameObject) {
        if (gameObject == null) return DefaultPosition;
        RectTransform rectTransform = (RectTransform)gameObject.transform;
        Vector3 targetPosition;
        switch (target.TargetPositionType) {
            case EPosition.Global:
                targetPosition = rectTransform.position;
                break;
            case EPosition.Local:
                targetPosition = rectTransform.localPosition;
                break;
            case EPosition.Anchored:
                targetPosition = rectTransform.anchoredPosition;
                break;
            default:
                targetPosition = rectTransform.anchoredPosition;
                break;
        }
        targetPosition += (Vector3)target.Offset;
        return targetPosition;
    }

    [Button]
    public void Move() => Move(GetPosition(target.GameObject));

    public void ChangeDuration(float duration) {
        target.CurrentMoveToDuration = duration;
    }

    public void MoveTo(GameObject gameObject) => MoveTo(gameObject, target.CurrentMoveToDuration);

    public void MoveTo(GameObject gameObject, float duration) {
        if (gameObject == null || target.GameObject == gameObject) return;
        target.GameObject = gameObject;
        target.CurrentMoveToDuration = duration;
        Move();
    }

    public void MoveTo(MoveToTarget target) {
        if (target.GameObject == null || this.target.GameObject == target.GameObject) return;
        this.target = target;
        Move();
    }

    public enum EPosition {
        Global,
        Local,
        Anchored
    }

    [Serializable]
    public class MoveToTarget {
        public GameObject GameObject;
        public EPosition TargetPositionType = EPosition.Anchored;
        public Vector2 Offset;
        public float MoveToDuration = 1f;
        [Description("Seconds")]
        public float InitialMoveToDuration = 1f;
        [HideInInspector] public float CurrentMoveToDuration = 0f;
    }
}
