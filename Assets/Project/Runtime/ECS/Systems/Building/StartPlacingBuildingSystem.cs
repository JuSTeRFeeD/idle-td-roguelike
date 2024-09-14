using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Views;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Features.Widgets;
using Project.Runtime.Scriptable.Card.Perks;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
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
                ref var data = ref entity.GetComponent<PlacingBuildingCard>();
                data.CellEntity.Dispose();
                entity.Dispose();
                _handsManager.SetPlacingEnabledEnabled(false);
                _handsManager.SetIsCardDrag(false);
            }
            
            Time.timeScale = 1f;
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

            Time.timeScale = 0.1f;
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