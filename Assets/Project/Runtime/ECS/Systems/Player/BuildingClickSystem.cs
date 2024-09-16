using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Views;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.BuildingsManagement;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class BuildingClickSystem : ISystem
    {
        [Inject] private CameraController _cameraController;
        [Inject] private PanelsManager _panelsManager;
        [Inject] private BuildingManagementPanel _buildingManagementPanel;
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private MapManager _mapManager;
        [Inject] private WorldSetup _worldSetup;

        public World World { get; set; }

        private Entity _selectedEntity;

        private Filter _unitsFilter;
        
        private Filter _stoneStorageFilter;
        private Filter _woodStorageFilter;
        private Stash<WoodStorage> _woodStorageStash;
        private Stash<StoneStorage> _stoneStorageStash;

        public void OnAwake()
        {
            _cameraController.OnEntityViewClick += OnClickEntityView;
            _buildingManagementPanel.OnCloseClick += CloseManagement;
            Debug.Log("Building click system inited");

            _unitsFilter = World.Filter
                .With<UnitTag>()
                .With<Owner>()
                .Build();

            _woodStorageFilter = World.Filter
                .With<WoodStorage>()
                .Build();
            _stoneStorageFilter = World.Filter
                .With<StoneStorage>()
                .Build();
            _woodStorageStash = World.GetStash<WoodStorage>();
            _stoneStorageStash = World.GetStash<StoneStorage>();
        }

        private void CloseManagement()
        {
            _panelsManager.SetPanel(PanelType.None);
            if (!_selectedEntity.IsNullOrDisposed() && _selectedEntity.Has<RadiusViewEntity>())
            {
                _selectedEntity.GetComponent<RadiusViewEntity>().Entity.Dispose();
            }
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

            // Radius view
            if (_selectedEntity.Has<AttackRangeRuntime>())
            {
                var radiusEntity = World.CreateEntity();
                radiusEntity.InstantiateView(
                    _worldSetup.RadiusView, 
                    _selectedEntity.ViewPosition(),
                    Quaternion.identity);
                radiusEntity.SetComponent(new Owner
                {
                    Entity = _selectedEntity
                });
                _selectedEntity.SetComponent(new RadiusViewEntity
                {
                    Entity = radiusEntity
                });
            }
            
            // Setup panel widgets
            _panelsManager.SetPanel(PanelType.TowerManagement);

            _cameraController.SetPosition(_selectedEntity.ViewPosition() - new Vector3(2, 0, 2));
            
            if (entity.Has<BuildingTag>() && _buildingsDatabase.TryGetById(
                    entity.GetComponent<BuildingTag>().BuildingConfigId, out var config))
            {
                // Upgradable
                if (config is UpgradableTowerConfig upgradableTowerConfig)
                {
                    var gridPos = GridUtils.ConvertWorldToGridPos(entity.ViewPosition());
                    var building = _mapManager.Buildings[gridPos];
                    _buildingManagementPanel.SetTitleAndLevel(config.Title, building.lvl, (float)building.lvl / upgradableTowerConfig.UpgradeLevels);
                    
                    // Upgrade
                    if (building.lvl < upgradableTowerConfig.UpgradeLevels)
                    {
                        var prices = upgradableTowerConfig.UpgradePrices[building.lvl]; 
                        _buildingManagementPanel.AddTowerWidget(OnClickTowerUpgrade);
                        _buildingManagementPanel.SetUpgradeTowerWidgetPrices(prices.woodPrice, prices.stonePrice);
                    }
                }
                else
                {
                    _buildingManagementPanel.SetTitle(config.Title);
                }
                
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
            if (entity.Has<UnitsOwnedTag>())
            {
                _buildingManagementPanel.AddUnitManagementWidget();
            }

            // Attack towers stats
            if (entity.Has<AttackDamageRuntime>() && 
                entity.Has<AttackCooldownRuntime>() && 
                entity.Has<AttackRangeRuntime>() &&
                entity.Has<HealthDefault>())
            {
                _buildingManagementPanel.AddStatsWidget();
            }
            if ((entity.Has<CriticalChanceRuntime>() && entity.Has<CriticalDamageRuntime>()) || 
                entity.Has<TowerWithBouncingProjectileRuntime>() || 
                entity.Has<SplashDamage>())
            {
                _buildingManagementPanel.AddStatsWidget(1);
            }
            
        }

        private void OnClickTowerUpgrade()
        {
            if (_selectedEntity.IsNullOrDisposed()) return;
            if (!_selectedEntity.Has<BuildingTag>()) return;

            var buildingId = _selectedEntity.GetComponent<BuildingTag>().BuildingConfigId;
            if (!_buildingsDatabase.TryGetById(buildingId, out var config)) return;

            // Get prices
            if (config is not UpgradableTowerConfig upgradableTowerConfig) return;
            var gridPos = GridUtils.ConvertWorldToGridPos(_selectedEntity.ViewPosition());
            var building = _mapManager.Buildings[gridPos];
            var prices = upgradableTowerConfig.UpgradePrices[building.lvl];
            
            // Decreasing resources
            TakeResourcesFromStorages(prices);

            // Spawn building over other building to auto merge in building systems
            var request = World.CreateEntity();
            request.SetComponent(new PlacingBuildingCard
            {
                BuildingConfig = config,
                CurrentPosition = _selectedEntity.ViewPosition(),
                IsCollisionDetected = true,
                IsMergeCollisionDetected = true
            });
            request.SetComponent(new PlaceBuildingCardRequest());

            // Redraw ui
            _buildingManagementPanel.SetTitleAndLevel(
                config.Title, 
                building.lvl + 1, 
                (float)(building.lvl + 1) / upgradableTowerConfig.UpgradeLevels);
            if (building.lvl + 1 >= upgradableTowerConfig.UpgradeLevels)
            {
                _buildingManagementPanel.DestroyUpgradeTowerWidget();
            }
            else
            {
                var nextPrices = upgradableTowerConfig.UpgradePrices[building.lvl + 1];
                _buildingManagementPanel.SetUpgradeTowerWidgetPrices(nextPrices.woodPrice, nextPrices.stonePrice);
            }
        }

        private void TakeResourcesFromStorages(UpgradableTowerConfig.UpgradePrice prices)
        {
            while (prices.woodPrice > 0)
            {
                foreach (var entity in _woodStorageFilter)
                {
                    ref var storage = ref _woodStorageStash.Get(entity);
                    if (storage.Current >= prices.woodPrice)
                    {
                        storage.Current -= prices.woodPrice;
                        prices.woodPrice = 0;
                        entity.SafeRemove<WoodStorageFullTag>();
                    }
                    else if (storage.Current > 0)
                    {
                        var possible = prices.woodPrice - storage.Current;
                        storage.Current -= possible;
                        prices.woodPrice -= possible;
                        entity.SafeRemove<WoodStorageFullTag>();
                    }

                }
            }

            while (prices.stonePrice > 0)
            {
                foreach (var entity in _stoneStorageFilter)
                {
                    ref var storage = ref _stoneStorageStash.Get(entity);
                    if (storage.Current >= prices.stonePrice)
                    {
                        storage.Current -= prices.stonePrice;
                        prices.stonePrice = 0;
                        entity.SafeRemove<StoneStorageFullTag>();
                    }
                    else if (storage.Current > 0)
                    {
                        var possible = prices.stonePrice - storage.Current;
                        storage.Current -= possible;
                        prices.stonePrice -= possible;
                        entity.SafeRemove<StoneStorageFullTag>();
                    }
                }
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (_selectedEntity.IsNullOrDisposed()) return;

            // Отображение в UI сколько использует юнитов этот тавер
            if (_selectedEntity.Has<UnitsOwnedTag>())
            {
                ref var minerUnitsOwned = ref _selectedEntity.GetComponent<UnitsOwnedTag>();
                var used = _unitsFilter.GetLengthSlow();
                
                _buildingManagementPanel.SetUnitsWidgetValues(
                    used,
                    minerUnitsOwned.CurrentCapacity,
                    minerUnitsOwned.Capacity.max
                );
            }
            
            // Отображение для грейда сколько имеем ресов
            if (_selectedEntity.Has<BuildingTag>())
            {
                var stoneAmount = 0;
                var woodAmount = 0;
                foreach (var entity in _woodStorageFilter)
                {
                    woodAmount += _woodStorageStash.Get(entity).Current;
                }
                foreach (var entity in _stoneStorageFilter)
                {
                    stoneAmount += _stoneStorageStash.Get(entity).Current;
                }
                _buildingManagementPanel.SetUpgradeTowerWidgetTotalResourcesAmount(woodAmount, stoneAmount);
            }
            
            // Attack towers stats
            if (_selectedEntity.Has<AttackDamageRuntime>() && 
                _selectedEntity.Has<AttackCooldownRuntime>() && 
                _selectedEntity.Has<AttackRangeRuntime>() &&
                _selectedEntity.Has<HealthDefault>())
            {
                _buildingManagementPanel.SetStatsWidgetText(new List<string>
                {
                    $"Damage {_selectedEntity.GetComponent<AttackDamageRuntime>().Value:##.#}", 
                    $"Range {_selectedEntity.GetComponent<AttackRangeRuntime>().Value:##.#}", 
                    $"Health: {_selectedEntity.GetComponent<HealthDefault>().Value:##.#}", 
                    $"Attack Speed: {(1f / _selectedEntity.GetComponent<AttackCooldownRuntime>().Value):F1}", 
                });
            }
            if (_selectedEntity.Has<CriticalChanceRuntime>() && 
                _selectedEntity.Has<CriticalDamageRuntime>())
            {
                var stats = new List<string>(4)
                {
                    $"Crit. Chance: {_selectedEntity.GetComponent<CriticalChanceRuntime>().Value * 100:##.#}%",
                    $"Crit. Damage: {_selectedEntity.GetComponent<CriticalDamageRuntime>().Value * 100:##.#}%",
                };
                if (_selectedEntity.Has<TowerWithBouncingProjectileRuntime>())
                {
                    stats.Add($"Hit Bounces: {_selectedEntity.GetComponent<TowerWithBouncingProjectileRuntime>().Bounces}");
                }
                if (_selectedEntity.Has<SplashDamage>())
                {
                    ref readonly var splashDamage = ref _selectedEntity.GetComponent<SplashDamage>();
                    stats.Add($"Splash Damage: {splashDamage.PercentFromDamage:##.#}%");
                    if (stats.Count < 4)
                    {
                        stats.Add($"Splash Radius: {splashDamage.Radius:##.#}");
                    }
                }
                _buildingManagementPanel.SetStatsWidgetText(stats, 1);
            }
        }

        public void Dispose()
        {
        }
    }
}