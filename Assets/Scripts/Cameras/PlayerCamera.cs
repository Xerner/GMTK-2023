﻿using Assets.Scripts.Cells;
using Assets.Scripts.Extensions;
using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Cameras
{
    public class PlayerCamera : MonoBehaviour
    {
        [Range(2, 20)]
        public float cameraTargetDivider;

        [SerializeField]
        private GameObject _focusPoint;
        [SerializeField]
        private Camera _mainCamera;
        [SerializeField]
        private Cell _playerCell;

        private void Start()
        {
            this.EnsureHasReference(ref _playerCell);
            this.EnsureHasReference(ref _mainCamera);
            this.EnsureHasReference(ref _focusPoint);
        }

        // Update is called once per frame
        void Update()
        {
            var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var cameraTargetPosition = (mousePosition + (cameraTargetDivider - 1) * _playerCell.transform.position) / cameraTargetDivider;
            _focusPoint.transform.position = cameraTargetPosition;
        }
    }
}