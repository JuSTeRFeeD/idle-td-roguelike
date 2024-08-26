using Project.Runtime.ECS.Components;
using Project.Runtime.Features.CameraControl;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems
{
    public class CameraMoveSystem : ISystem
    {
        [Inject] private CameraController _cameraController;

        private Filter _filter;

        private Vector3 _dragOrigin;
        private const float DragSpeed = 10f;
        
        public World World { get; set; }

        public void OnAwake()
        {
            _filter = World.Filter
                .With<PlacingBuilding>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_filter.IsNotEmpty()) return;

            if (Input.GetMouseButtonDown(0))
            {
                _dragOrigin = Input.mousePosition;
                return;
            }

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