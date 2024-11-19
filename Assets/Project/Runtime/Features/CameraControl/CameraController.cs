using System;
using Cinemachine;
using Project.Runtime.ECS.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Runtime.Features.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform target;

        public event Action<EntityView> OnEntityViewClick;

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
            virtualCamera.Follow = tempTarget;
            virtualCamera.LookAt = tempTarget;
        }
        
        public void ResetTarget()
        {
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
        }

        public void SetPosition(Vector3 position)
        {
            target.position = position;
        }

        private void ClickHandle()
        {
            if (!Input.GetMouseButtonDown(0) || !EventSystem.current) return;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;

            if (!hit.transform.TryGetComponent(out EntityView view)) return;

            OnEntityViewClick?.Invoke(view);
            Debug.Log("click on entity " + view.name);
        }
    }
}