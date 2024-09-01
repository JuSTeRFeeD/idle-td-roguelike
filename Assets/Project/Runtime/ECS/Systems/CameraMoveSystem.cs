using Project.Runtime.ECS.Components;
using Project.Runtime.Features.CameraControl;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Project.Runtime.ECS.Systems
{
    public class CameraMoveSystem : ISystem
    {
        [Inject] private CameraController _cameraController;

        private Filter _isBuildingFilter;

        private Vector3 _dragOrigin;
        private bool _isDrag; 
        private const float DragSpeed = 6f;
        
        public World World { get; set; }

        public void OnAwake()
        {
            _isBuildingFilter = World.Filter
                .With<PlacingBuildingCard>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // No need move camera while building
            if (_isBuildingFilter.IsNotEmpty()) return;

            if (Input.GetMouseButtonUp(0))
            {
                _isDrag = false;
            }
            
            if (!_isDrag)
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    _dragOrigin = Input.mousePosition;
                    _isDrag = true;
                }
            }

            if (!_isDrag) return;

            var difference = Vector3.zero;
            if (Input.GetMouseButton(0))
            {
                var currentMousePos = Input.mousePosition;
                difference = _dragOrigin - currentMousePos;
                _dragOrigin = currentMousePos;
            }

            var mainCameraTransform = _cameraController.MainCamera.transform;
            var forward = mainCameraTransform.forward * difference.y;
            var right = mainCameraTransform.right * difference.x;
            var move = forward + right;
            move.y = 0;

            var additionalSpeedByDist = difference.magnitude * .2f;
            var newPos = _cameraController.OriginTargetPosition + move.normalized * DragSpeed * deltaTime * additionalSpeedByDist;
            _cameraController.SetPosition(newPos);
        }

        public void Dispose()
        {
        }
    }
}