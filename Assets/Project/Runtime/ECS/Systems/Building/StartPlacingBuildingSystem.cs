using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Views;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Features.TimeManagement;
using Project.Runtime.Features.Widgets;
using Project.Runtime.Scriptable.Buildings;
using Project.Runtime.Scriptable.Card.Perks;
using Runtime.Features.Widgets;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class StartPlacingBuildingSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private CameraController _cameraController;
        [Inject] private HandsManager _handsManager;
        
        public World World { get; set; }

        private Filter _startPlaceBuildingFilter;
        private Filter _placingBuildingFilter;
        
        public void OnAwake()
        {
            _startPlaceBuildingFilter = World.Filter
                .With<StartPlaceBuildingCardRequest>()
                .Build();
            _placingBuildingFilter = World.Filter
                .With<PlacingBuildingCard>()
                .Build();

            _handsManager.OnCardUseStart += OnCardUseStart;
            _handsManager.OnCardUseCancel += OnCardUseCancel;
        }

        public void Dispose()
        {
            _handsManager.OnCardUseStart -= OnCardUseStart;
            _handsManager.OnCardUseCancel -= OnCardUseCancel;
        }

        private void OnCardUseCancel()
        {
            Debug.Log("Cancel use card building");
            foreach (var entity in _placingBuildingFilter)
            {
                if (entity.Has<RadiusViewEntity>())
                {
                    entity.GetComponent<RadiusViewEntity>().Entity.Dispose();
                }
                ref var data = ref entity.GetComponent<PlacingBuildingCard>();
                data.CellEntity.Dispose();
                entity.Dispose();
                _handsManager.SetPlacingEnabledEnabled(false);
                _handsManager.SetIsCardDrag(false);
            }
            
            TimeScale.SetNormalTimeScale();
        }

        // Drag карточки на поле
        // Создается реквест на начало строительства
        private void OnCardUseStart(CardWidget cardWidget)
        {
            if (_handsManager.IsCardDrag) return;
            
            foreach (var cardConfigPerk in cardWidget.CardConfig.Perks)
            {
                if (cardConfigPerk is GiveBuildingPerk buildingPerk)
                {
                    var placePosition = _cameraController.transform.position;
                    var mousePos = Input.mousePosition;
                    var ray = _cameraController.MainCamera.ScreenPointToRay(mousePos);
                    if (Physics.Raycast(ray, out var hit))
                    {
                        placePosition = hit.point;
                    }
                    placePosition.y = 0;

                    placePosition = GridUtils.Vec3ToCellPos(placePosition);
                    
                    var requestEntity = World.CreateEntity();
                    requestEntity.SetComponent(new StartPlaceBuildingCardRequest
                    {
                        BuildingConfig = buildingPerk.BuildingConfig,
                        StartPlacingPosition = placePosition,
                        CardConfigId = cardWidget.CardConfig.uniqueID
                    });
                    
                    Debug.Log("[StartPlacingBuildingSystem] Drag карты на поле, создался реквест на начало строительства");
                    
                    break;
                }
            }

            TimeScale.SetTimeScale(0.075f);
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _startPlaceBuildingFilter)
            {
                var data = entity.GetComponent<StartPlaceBuildingCardRequest>();
                
                var building = World.CreateEntity();
                building.InstantiateView(data.BuildingConfig.Prefab, data.StartPlacingPosition, Quaternion.identity);
                
                // Placing cell view
                var cellEntity = World.CreateEntity();
                cellEntity.SetComponent(new Owner { Entity = building });
                var view = cellEntity.InstantiateView<PlacingCellView>(_worldSetup.PlacingCellView, data.StartPlacingPosition, Quaternion.Euler(-90, 0, 0));
                view.SetSize(data.BuildingConfig.Size);
                
                // Radius view
                if (data.BuildingConfig is AttackTowerBuildingConfig attackTowerBuildingConfig)
                {
                    var radiusEntity = World.CreateEntity();
                    var radiusView = radiusEntity.InstantiateView<RadiusView>(
                        _worldSetup.RadiusView,
                        data.StartPlacingPosition,
                        Quaternion.identity);
                    radiusView.SetRadius(attackTowerBuildingConfig.AttackRange.min);
                    radiusEntity.SetComponent(new Owner
                    {
                        Entity = building
                    });
                    building.SetComponent(new RadiusViewEntity
                    {
                        Entity = radiusEntity
                    });
                }
                
                building.SetComponent(new PlacingBuildingCard
                {
                    CurrentPosition = data.StartPlacingPosition,
                    BuildingConfig = data.BuildingConfig,
                    CellEntity = cellEntity,
                    CardConfigId = data.CardConfigId
                });
                
                // Камера будет следовать за объектом который строим. По ощущениям даже может и можно юзнуть
                // _cameraController.OverrideTarget(preview.transform);
                
                entity.Dispose();
            }
        }
    }
}