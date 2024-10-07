using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Features.TimeManagement;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class PlacingBuildingSystem : ISystem
    {
        [Inject] private CameraController _cameraController;
        [Inject] private MapManager _mapManager;
        [Inject] private HandsManager _handsManager;
        
        public World World { get; set; }

        private Filter _filter;

        private const int GroundLayer = 1 << 3;
        
        private Vector3 _placePosition;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<PlacingBuildingCard>()
                .With<ViewEntity>()
                .Without<PlaceBuildingCardRequest>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            var mousePos = Input.mousePosition;
            
            foreach (var entity in _filter)
            {
                var isAnyCollisionDetected = false;
                var isMergeCollisionDetected = false;
                
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

                // Merge 
                if (gridPos.x >= 0 && gridPos.x < _mapManager.MapSize &&
                    gridPos.y >= 0 && gridPos.y < _mapManager.MapSize &&
                    placingBuilding.BuildingConfig is UpgradableTowerConfig upgradableTowerConfig)
                {
                    var buildingData = _mapManager.Buildings[gridPos];
                    if (buildingData != null)
                    {
                        if (buildingData.id == upgradableTowerConfig.uniqueID &&
                            buildingData.Entity.GetComponent<BuildingTag>().Level < upgradableTowerConfig.UpgradePrices.Length)
                        {
                            isMergeCollisionDetected = true;
                        }
                    }
                }
                // Collision
                isAnyCollisionDetected = _mapManager.CheckCollision(
                    gridPos.x, gridPos.y, buildingSize, buildingSize);
                
                placingBuilding.CurrentPosition = GridUtils.ConvertGridToWorldPos(gridPos);
                placingBuilding.IsCollisionDetected = isAnyCollisionDetected;
                placingBuilding.IsMergeCollisionDetected = isMergeCollisionDetected;
                entity.ViewTransform().position = _placePosition;
                
                // Put building
                if (!Input.GetMouseButtonUp(0)) continue;
                if (isAnyCollisionDetected && !isMergeCollisionDetected)
                {
                    // Cancel placing (same code in SpawnPlacingBuildingSystem.OnCardUseCancel)
                    if (entity.Has<RadiusViewEntity>())
                    {
                        entity.GetComponent<RadiusViewEntity>().Entity.Dispose();
                    }
                    placingBuilding.CellEntity.Dispose();
                    entity.Dispose();
                    _handsManager.SetPlacingEnabledEnabled(false);
                    _handsManager.SetIsCardDrag(false);
                    
                    TimeScale.SetNormalTimeScale();
                        
                    continue;
                }
                    
                entity.SetComponent(new PlaceBuildingCardRequest());
            }
        }

        public void Dispose()
        {
        }
    }
}