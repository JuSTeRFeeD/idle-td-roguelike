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
        [Inject] private ResourceCounter _resourceCounter;

        public World World { get; set; }

        private Entity _selectedEntity;
        private Filter _unitsFilter;
        
        public void OnAwake()
        {
            _cameraController.OnEntityViewClick += OnClickEntityView;
            _buildingManagementPanel.OnCloseClick += CloseManagement;
            Debug.Log("Building click system inited");

            _unitsFilter = World.Filter
                .With<UnitTag>()
                .With<Owner>()
                .Build();
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

            _cameraController.SetPosition(_selectedEntity.ViewPosition() - new Vector3(4, 0, 4));
            
            if (entity.Has<BuildingTag>())
            {
                ref readonly var buildingTag = ref entity.GetComponent<BuildingTag>();
                _buildingsDatabase.TryGetById(buildingTag.BuildingConfigId, out var config);
                
                // Upgradable
                if (config is UpgradableTowerConfig upgradableTowerConfig)
                {
                    // Upgrade
                    if (buildingTag.Level < upgradableTowerConfig.UpgradePrices.Length)
                    {
                        var prices = upgradableTowerConfig.UpgradePrices[buildingTag.Level]; 
                        _buildingManagementPanel.AddTowerWidget(OnClickTowerUpgrade);
                        _buildingManagementPanel.SetUpgradeTowerWidgetPrices(prices.woodPrice, prices.stonePrice);
                    }
                    
                    _buildingManagementPanel.SetTitleAndLevel(
                        config.Title,
                        buildingTag.Level, 
                        (float)buildingTag.Level / upgradableTowerConfig.UpgradePrices.Length);
                    Debug.Log($"Click upgradable building {buildingTag.Level} / {upgradableTowerConfig.UpgradePrices.Length}");
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
                entity.Has<AttackRangeRuntime>() ||
                entity.Has<HealthDefault>())
            {
                _buildingManagementPanel.AddStatsWidget();
            }
            if ((entity.Has<CriticalChanceRuntime>() && entity.Has<CriticalDamageRuntime>()) || 
                entity.Has<TowerWithBouncingProjectileRuntime>() || 
                entity.Has<SplashDamageRuntime>())
            {
                _buildingManagementPanel.AddStatsWidget(1);
            }

            _buildingManagementPanel.ResetScroll();
        }

        private void OnClickTowerUpgrade()
        {
            if (_selectedEntity.IsNullOrDisposed()) return;
            _selectedEntity.AddComponent<UpgradeBuildingRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_selectedEntity.IsNullOrDisposed()) return;
            
            // TODO: optimize cuz allocations each frame for redraw text & ui
            
            if (_selectedEntity.Has<BuildingUpgraded>())
            {
                Debug.Log("[BuildingClickSystem] Building upgraded event handled");
                ref readonly var buildingTag = ref _selectedEntity.GetComponent<BuildingTag>();
                var buildingId = buildingTag.BuildingConfigId;
                if (_buildingsDatabase.TryGetById(buildingId, out var config) &&
                    config is UpgradableTowerConfig upgradableTowerConfig)
                {
                    // Redraw ui prices
                    _buildingManagementPanel.SetTitleAndLevel(
                        config.Title,
                        buildingTag.Level,
                        (float)buildingTag.Level / upgradableTowerConfig.UpgradePrices.Length);
                    if (buildingTag.Level >= upgradableTowerConfig.UpgradePrices.Length)
                    {
                        _buildingManagementPanel.DestroyUpgradeTowerWidget();
                    }
                    else
                    {
                        var nextPrices = upgradableTowerConfig.UpgradePrices[buildingTag.Level];
                        _buildingManagementPanel.SetUpgradeTowerWidgetPrices(nextPrices.woodPrice, nextPrices.stonePrice);
                    }
                }
                _selectedEntity.RemoveComponent<BuildingUpgraded>();
            }
            
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
                _buildingManagementPanel.SetUpgradeTowerWidgetTotalResourcesAmount(_resourceCounter.WoodAmount, _resourceCounter.StoneAmount);
            }
            
            // Attack towers stats
            if (_selectedEntity.Has<AttackDamageRuntime>() && 
                _selectedEntity.Has<AttackCooldownRuntime>() && 
                _selectedEntity.Has<AttackRangeRuntime>() &&
                _selectedEntity.Has<HealthDefault>())
            {
                var list = new List<string>
                {
                    $"Урон {_selectedEntity.GetComponent<AttackDamageRuntime>().Value:#0.#}",
                    $"Радиус {_selectedEntity.GetComponent<AttackRangeRuntime>().Value:#0.#}",
                    $"Здоровье {_selectedEntity.GetComponent<HealthDefault>().Value:#0.#}",
                };
                if (_selectedEntity.Has<BombTowerTag>())
                {
                    list.Add($"Перезарядка {_selectedEntity.GetComponent<AttackCooldownRuntime>().Value:#0.#}");
                }
                else
                {
                    list.Add($"Атак в сек. {1f / _selectedEntity.GetComponent<AttackCooldownRuntime>().Value:#0.#}");
                }
                _buildingManagementPanel.SetStatsWidgetText(list);
            }
            else if (_selectedEntity.Has<HealthDefault>())
            {
                var list = new List<string> { $"Здоровье {_selectedEntity.GetComponent<HealthDefault>().Value:#0.#}" };
                _buildingManagementPanel.SetStatsWidgetText(list);
            }
            
            if (_selectedEntity.Has<CriticalChanceRuntime>() && 
                _selectedEntity.Has<CriticalDamageRuntime>())
            {
                var stats = new List<string>(4)
                {
                    $"Крит. шанс {_selectedEntity.GetComponent<CriticalChanceRuntime>().Value * 100:#0.#}%",
                    $"Крит. урон {_selectedEntity.GetComponent<CriticalDamageRuntime>().Value * 100:#0.#}%",
                };
                if (_selectedEntity.Has<TowerWithBouncingProjectileRuntime>())
                {
                    stats.Add($"Отскоки {_selectedEntity.GetComponent<TowerWithBouncingProjectileRuntime>().Bounces}");
                }
                if (_selectedEntity.Has<SplashDamageRuntime>())
                {
                    ref readonly var splashDamage = ref _selectedEntity.GetComponent<SplashDamageRuntime>();
                    stats.Add($"Сплеш урон {splashDamage.PercentFromDamage * 100:#0.#}%");
                    if (stats.Count < 4)
                    {
                        stats.Add($"Сплеш радиус {splashDamage.Radius:#0.#}");
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