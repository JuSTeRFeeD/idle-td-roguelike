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
                    // Для корректного размещения 
                    _placePosition -= new Vector3(GridUtils.CellHalf, 0, GridUtils.CellHalf);
                }

                var buildingSize = placingBuilding.BuildingConfig.Size;
                var gridPos = GridUtils.ConvertWorldToGridPos(_placePosition);
                _placePosition = GridUtils.ConvertGridToWorldPos(gridPos) + GridUtils.BuildingSizeOffset(buildingSize);

                isAnyCollisionDetected = _mapManager.CheckCollision(
                    gridPos.x, gridPos.y, buildingSize, buildingSize);
                
                placingBuilding.CurrentPosition = GridUtils.ConvertGridToWorldPos(gridPos);
                placingBuilding.IsCollisionDetected = isAnyCollisionDetected;
                entity.ViewTransform().position = _placePosition;
                
                // Put building
                if (Input.GetMouseButtonUp(0))
                {
                    if (isAnyCollisionDetected)
                    {
                        continue;
                    }
                    entity.SetComponent(new PlaceBuildingCardRequest());
                }
            }
        }

        public void Dispose()
        {
        }
    }
}