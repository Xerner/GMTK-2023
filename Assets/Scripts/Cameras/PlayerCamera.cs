using Assets.Scripts.Cells;
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

        private CinemachineVirtualCamera _camera;

        //[SerializeField]
        //private GameObject _focusPoint;
        //[SerializeField]
        //private Camera _mainCamera;
        private Cell controlledCell;

        private void Start()
        {
            Player.Instance.OnCellSwap += UpdateControlledCell;
            _camera = GetComponent<CinemachineVirtualCamera>();
            //this.EnsureHasReference(ref _mainCamera);
            //this.EnsureHasReference(ref _focusPoint);
        }

        public void UpdateControlledCell(Cell cell)
        {
            controlledCell = cell;
            _camera.Follow = cell.transform;
        }

        void Update()
        {
            //var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            //var cameraTargetPosition = (mousePosition + (cameraTargetDivider - 1) * controlledCell.transform.position) / cameraTargetDivider;
            //_focusPoint.transform.position = cameraTargetPosition;

            //var camPosition = _mainCamera.transform.position;
            //camPosition.z = -10;
            //_mainCamera.transform.position = camPosition;
        }
    }
}