using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
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

        private const int GroundLayer = 1 << 3;
        
        private Vector3 _placePosition;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<PlacingBuildingCard>()
                .With<ViewEntity>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            var mousePos = Input.mousePosition;
            
            foreach (var entity in _filter)
            {
                var isAnyCollisionDetected = false;
                ref var placingBuilding = ref entity.GetComponent<PlacingBuildingCard>();
                    
                var ray = _cameraController.MainCamera.ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray, out var hit, 100f, GroundLayer))
                {
                    _placePosition = hit.point;
                    _placePosition.y = 0;
                }

                var additionalOffset = Vector3.zero;
                var buildingSize = placingBuilding.BuildingConfig.Size;
                if (buildingSize != Vector2Int.one)
                {
                    additionalOffset = new Vector3(
                        buildingSize.x / 2f * GridUtils.CellHalf,
                        0,
                        buildingSize.y / 2f * GridUtils.CellHalf);   
                }
                _placePosition = GridUtils.ConvertGridToWorldPos(GridUtils.ConvertWorldToGridPos(_placePosition)) 
                                 + additionalOffset;

                var gridPos = GridUtils.ConvertWorldToGridPos(placingBuilding.CurrentPosition);
                isAnyCollisionDetected = _mapManager.CheckCollision(gridPos.x, gridPos.y, buildingSize.x, buildingSize.y);
                
                placingBuilding.CurrentPosition = _placePosition;
                placingBuilding.IsCollisionDetected = isAnyCollisionDetected;
                entity.ViewTransform().position = _placePosition;
                
                // TODO: place building throw UI for mobile
                // Put building
                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("Place building");
                    if (isAnyCollisionDetected) continue;
                    entity.SetComponent(new PlaceBuildingCardRequest());
                }
            }
        }

        public void Dispose()
        {
        }
    }
}