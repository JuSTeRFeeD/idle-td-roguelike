using System;
using Cinemachine;
using Project.Runtime.ECS.Views;
using UnityEngine;

namespace Project.Runtime.Features.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform target;

        public event Action<EntityView> OnEntityViewClick; // todo handle in system this event

        public Camera MainCamera => mainCamera;
        public Vector3 OriginTargetPosition => target.position;

        private void Start()
        {
            ResetTarget();
        }

        private void Update()
        {
            ClickHandle();
        }

        public void OverrideTarget(Transform tempTarget)
        {
            _virtualCamera.Follow = tempTarget;
            _virtualCamera.LookAt = tempTarget;
        }
        
        public void ResetTarget()
        {
            _virtualCamera.Follow = target;
            _virtualCamera.LookAt = target;
        }

        public void SetPosition(Vector3 position)
        {
            target.position = position;
        }

        private void ClickHandle()
        {
            if (!Input.GetMouseButtonDown(0)) return;

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;

            if (!hit.transform.TryGetComponent(out EntityView view)) return;

            OnEntityViewClick?.Invoke(view);
            Debug.Log("click on entity " + view.name);
        }
    }
}