using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.CameraControl;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    public class PlacingBuildingSystem : ISystem
    {
        [Inject] private CameraController _cameraController;
        [Inject] private MapManager _mapManager;
        
        public World World { get; set; }

        private Filter _filter;
        
        private const float MoveSpeed = 12f;
        private Vector3 _dragOrigin;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<PlacingBuilding>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
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
            
            foreach (var entity in _filter)
            {
                var anyPartCollision = false;
                ref var placingBuilding = ref entity.GetComponent<PlacingBuilding>();
                if (!placingBuilding.Preview) continue;
                    
                var mainCameraTransform = _cameraController.MainCamera.transform;
                var forward = mainCameraTransform.forward * difference.y;
                var right = mainCameraTransform.right * difference.x;
                var move = forward + right;
                move.y = 0;

                var additionalSpeedByDist = difference.magnitude * .2f;
                placingBuilding.CurrentPosition += move.normalized * MoveSpeed * deltaTime * additionalSpeedByDist;
                placingBuilding.Preview.transform.position = placingBuilding.CurrentPosition;

                // TODO: place building throw UI for mobile
                // Put building
                if (Input.GetMouseButtonDown(1))
                {
                    if (anyPartCollision) continue;
                    entity.SetComponent(new PlaceBuildingRequest());
                }
            }
        }

        public void Dispose()
        {
        }
    }
}