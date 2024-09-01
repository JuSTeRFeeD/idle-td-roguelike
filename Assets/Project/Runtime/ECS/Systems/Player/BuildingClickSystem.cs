using System;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Views;
using Project.Runtime.Features.BuildingsManagement;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    public class BuildingClickSystem : ISystem
    {
        [Inject] private CameraController _cameraController;
        [Inject] private PanelsManager _panelsManager;
        [Inject] private BuildingManagementPanel _buildingManagementPanel;
        
        public World World { get; set; }

        private Entity _selectedEntity;

        private Filter _lumberjackUnitsFilter;
        private Filter _minerUnitsFilter;
        private Filter _chillingUnitsFilter;
        
        public void OnAwake()
        {
            _cameraController.OnEntityViewClick += OnClickEntityView;
            _buildingManagementPanel.OnCloseClick += CloseManagement;
            Debug.Log("Building click system inited");

            _lumberjackUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<LumberjackTag>()
                .With<Owner>()
                .Build();
            
            _minerUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<MinerTag>()
                .With<Owner>()
                .Build();

            _chillingUnitsFilter = World.Filter
                .With<UnitTag>()
                .Without<MinerTag>()
                .Without<LumberjackTag>()
                .Without<Owner>()
                .Build();
        }

        private void CloseManagement()
        {
            _panelsManager.SetPanel(PanelType.None);
            _selectedEntity = null;
        }

        private void OnClickEntityView(EntityView entityView)
        {
            var entity = entityView.Entity;
            if (entity.IsNullOrDisposed() || !entity.Has<BuildingTag>())
            {
                _selectedEntity = null;
                return;
            }
            if (entity == _selectedEntity) return;

            _selectedEntity = entity;
            _panelsManager.SetPanel(PanelType.TowerManagement);
            
            // Title
            if (entity.Has<BaseTowerTag>())
            {
                _buildingManagementPanel.SetTitleAndLevel("Chabumba", 1, 0.1f);
            }

            // Storage Info's
            if (entity.Has<WoodStorage>())
            {
                _buildingManagementPanel.AddStorageInfoWidget(ResourceType.Stone, entity);
            }
            if (entity.Has<StoneStorage>())
            {
                _buildingManagementPanel.AddStorageInfoWidget(ResourceType.Wood, entity);
            }
            
            // Units management
            if (entity.Has<LumberjackUnitsOwnedTag>())
            {
                _buildingManagementPanel.AddUnitManagementWidget(
                    UnitType.Lumberjack, 
                    () => RemoveUnitClick(UnitType.Lumberjack),
                    () => AddUnitClick(UnitType.Lumberjack));
            }
            if (entity.Has<MinerUnitsOwnedTag>())
            {
                _buildingManagementPanel.AddUnitManagementWidget(
                    UnitType.Miner, 
                    () => RemoveUnitClick(UnitType.Miner),
                    () => AddUnitClick(UnitType.Miner));
            }
        }

        private void AddUnitClick(UnitType unitType)
        {
            // TODO: handle building unit limits 
            
            foreach (var entity in _chillingUnitsFilter)
            {
                entity.AddComponent<FindResourceRequest>();
                entity.SetComponent(new Owner
                {
                    Entity = _selectedEntity
                });
                
                switch (unitType)
                {
                    case UnitType.Lumberjack:
                        entity.AddComponent<LumberjackTag>();
                        break;
                    case UnitType.Miner:
                        entity.AddComponent<MinerTag>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
                }

                break;
            }
        }

        private void RemoveUnitClick(UnitType unitType)
        {
            switch (unitType)
            {
                case UnitType.Lumberjack:
                    RemoveUnit(unitType, _lumberjackUnitsFilter);
                    break;
                case UnitType.Miner:
                    RemoveUnit(unitType, _minerUnitsFilter);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
            }
        }

        private void RemoveUnit(UnitType unitType, Filter unitFilter)
        {
            foreach (var entity in unitFilter)
            {
                if (entity.Owner() != _selectedEntity) continue;
                
                // clearing unit entity
                entity.RemoveComponent<Owner>();
                entity.GetComponent<UnitBackpack>().Amount = 0;
                entity.SafeRemove<MoveToResource>();
                entity.SafeRemove<MoveToStorage>();
                entity.SafeRemove<FindResourceRequest>();
                entity.SafeRemove<FindStorageRequest>();
                if (entity.Has<Gathering>())
                {
                    ref var gathering = ref entity.GetComponent<Gathering>();
                    if (!gathering.TargetResource.IsNullOrDisposed())
                    {
                        gathering.TargetResource.Dispose();
                    }
                    if (!gathering.ProgressEntity.IsNullOrDisposed())
                    {
                        gathering.ProgressEntity.Dispose();
                    }
                    entity.RemoveComponent<Gathering>();
                }
                
                switch (unitType)
                {
                    case UnitType.Lumberjack:
                        entity.RemoveComponent<LumberjackTag>();
                        break;
                    case UnitType.Miner:
                        entity.RemoveComponent<MinerTag>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null);
                }

                break;
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_selectedEntity.IsNullOrDisposed()) return;
            
            

            // Отображение в UI сколько использует юнитов этот тавер
            
            if (_selectedEntity.Has<LumberjackUnitsOwnedTag>())
            {
                ref var lumberjacksUnitsOwned = ref _selectedEntity.GetComponent<LumberjackUnitsOwnedTag>();
                var used = 0;
                foreach (var entity in _lumberjackUnitsFilter)
                {
                    if (entity.Owner() == _selectedEntity)
                    {
                        used++;
                    }
                }
                _buildingManagementPanel.SetUnitsWidgetValues(
                    UnitType.Lumberjack,
                    used, 
                    lumberjacksUnitsOwned.CurrentCapacity, 
                    lumberjacksUnitsOwned.Capacity.max);
            }
            
            if (_selectedEntity.Has<MinerUnitsOwnedTag>())
            {
                ref var minerUnitsOwned = ref _selectedEntity.GetComponent<MinerUnitsOwnedTag>();
                var used = 0;
                foreach (var entity in _minerUnitsFilter)
                {
                    if (entity.Owner() == _selectedEntity)
                    {
                        used++;
                    }
                }
                _buildingManagementPanel.SetUnitsWidgetValues(
                    UnitType.Miner,
                    used, 
                    minerUnitsOwned.CurrentCapacity, 
                    minerUnitsOwned.Capacity.max);
            }
        }

        public void Dispose()
        {
        }
    }
}