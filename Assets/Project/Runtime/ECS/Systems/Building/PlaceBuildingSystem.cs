using System;
using DG.Tweening;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Views;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    public class PlaceBuildingSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private VfxSetup _vfxSetup;
        [Inject] private MapManager _mapManager;
        [Inject] private CameraController _cameraController;
        [Inject] private HandsManager _handsManager;
        [Inject] private InventoryStorage _inventoryStorage;
        
        public World World { get; set; }

        private Filter _filter;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<PlaceBuildingCardRequest>()
                .With<PlacingBuildingCard>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                _handsManager.SetPlacingEnabledEnabled(false);
                _handsManager.SetIsCardDrag(false);
                Time.timeScale = 1f;

                var isSystemAction = entity.Has<SystemActionTag>();
                var placingBuilding = entity.GetComponent<PlacingBuildingCard>();
                var gridPos = GridUtils.ConvertWorldToGridPos(placingBuilding.CurrentPosition);
                EntityView view;
                
                // Merge
                if (placingBuilding.IsMergeCollisionDetected) 
                {
                    if (placingBuilding.BuildingConfig is not UpgradableTowerConfig towerConfig)
                    {
                        Debug.LogError("[MapManager] Lol its not upgradable tower");
                        return;
                    }

                    var building = _mapManager.UpgradeBuilding(towerConfig, gridPos);
                    if (building == null)
                    {
                        Debug.LogError("Нужно обработать если building null");
                    }
                    
                    // change view lvl
                    view = building.Entity.GetComponent<ViewEntity>().Value;
                    var towerView = (AttackTowerView)view;
                    towerView.TowerViewUpgrades.SetLevel(building.lvl);
                    UpgradeBuildingEntity(building.Entity, towerConfig, building.lvl);

                    VfxPool.Spawn(_vfxSetup.TowerLevelUpVfx, towerView.transform.position);
                }
                // Put new
                else 
                {
                    var buildingEntity = World.CreateEntity();
                    view = _mapManager.PutBuilding(
                        placingBuilding.BuildingConfig, 
                        gridPos,
                        Quaternion.identity, 
                        buildingEntity);
                    buildingEntity.LinkView(view);

                    if (view is AttackTowerView towerView)
                    {
                        towerView.TowerViewUpgrades.SetLevel(0);
                    }
                        
                    SetupEntity(buildingEntity, placingBuilding.BuildingConfig, view);

                    if (!isSystemAction)
                    {
                        VfxPool.Spawn(_vfxSetup.PutTowerVfx, view.transform.position);
                    }
                }

                if (!isSystemAction)
                {
                    view.transform.DOKill(true);
                    view.transform
                        .DOPunchScale(Vector3.up * .25f, 0.25f, 10, 2f)
                        .SetLink(view.gameObject);
                }

                // Reset when dragged from inventory (но если мы грейдили тавер то не нужно, собственно и не будет)
                if (placingBuilding.CardConfigId != null)
                {
                    _cameraController.ResetTarget();
                }

                // Removing card from list
                if (placingBuilding.CardConfigId != null)
                {
                    _inventoryStorage.RemoveCard(placingBuilding.CardConfigId);
                }
                
                placingBuilding.CellEntity?.Dispose();
                entity.Dispose();
            }
        }

        private void UpgradeBuildingEntity(Entity buildingEntity, UpgradableTowerConfig buildingConfig, int level)
        {
            var maxLevel = buildingConfig.UpgradeLevels;
            switch (buildingConfig)
            {
                case AttackTowerBuildingConfig attackTower:
                    // HP
                    ref var currentHealth = ref buildingEntity.GetComponent<HealthCurrent>().Value;
                    ref var defaultHealth = ref buildingEntity.GetComponent<HealthDefault>().Value;
                    var prevHealthPercent = currentHealth / defaultHealth;
                    defaultHealth = attackTower.Health.Evaluate(level, maxLevel);
                    currentHealth = defaultHealth * prevHealthPercent;
                    // Other stats
                    buildingEntity.GetComponent<AttackDamage>().Value = attackTower.Damage.Evaluate(level, maxLevel);
                    buildingEntity.GetComponent<AttackCooldown>().Value = attackTower.AttackCooldown.Evaluate(level, maxLevel);
                    buildingEntity.GetComponent<AttackRange>().Value = attackTower.AttackRange.Evaluate(level, maxLevel);
                    break;
            }
        }

        private void SetupEntity(in Entity buildingEntity, in BuildingConfig buildingConfig, in EntityView view)
        {
            switch (buildingConfig)
            {
                case BaseBuildingConfig baseBuilding:
                {
                    buildingEntity.SetComponent(new BuildingTag
                    {
                        BuildingConfigId = buildingConfig.uniqueID,
                        Size = buildingConfig.Size
                    });
                    buildingEntity.SetComponent(new BaseTowerTag());

                    buildingEntity.SetComponent(new HealthDefault
                    {
                        Value = baseBuilding.Health
                    });
                    buildingEntity.SetComponent(new HealthCurrent
                    {
                        Value = baseBuilding.Health,
                        GhostValue = baseBuilding.Health,
                    });
                    
                    buildingEntity.SetComponent(new UnitsOwnedTag
                    {
                        CurrentCapacity = baseBuilding.UnitsCapacity.min,
                        Capacity = baseBuilding.UnitsCapacity,
                    });
                    buildingEntity.SetComponent(new WoodStorage
                    {
                        Current = 0,
                        Max = baseBuilding.WoodStorageSize
                    });
                    buildingEntity.SetComponent(new StoneStorage
                    {
                        Current = 0,
                        Max = baseBuilding.StoneStorageSize
                    });
                    for (var i = 0; i < baseBuilding.UnitsCountDefault; i++)
                    {
                        var spawnUnitRequest = World.CreateEntity();
                        spawnUnitRequest.SetComponent(new SpawnUnitRequest
                        {
                            AtPosition = view.transform.position + Vector3.right * 3f,
                            ForTowerOwner = buildingEntity,
                        });
                    }

                    break;
                }

                case MapResourceConfig resource: 
                {
                    buildingEntity.SetComponent(new MapResourceTag());
                    buildingEntity.SetComponent(new HealthDefault
                    {
                        Value = resource.health
                    });
                    buildingEntity.SetComponent(new HealthCurrent
                    {
                        Value = resource.health,
                        GhostValue = resource.health
                    });
                    switch (resource.resourceType)
                    {
                        case ResourceType.Wood:
                            buildingEntity.SetComponent(new TreeTag());
                            break;
                        case ResourceType.Stone:
                            buildingEntity.SetComponent(new StoneTag());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    break;
                }

                case AttackTowerBuildingConfig attackTower:
                {
                    buildingEntity.SetComponent(new BuildingTag
                    {
                        BuildingConfigId = buildingConfig.uniqueID,
                        Size = buildingConfig.Size
                    });
                    buildingEntity.SetComponent(new AttackTowerTag());

                    buildingEntity.SetComponent(new HealthDefault
                    {
                        Value = attackTower.Health.min
                    });
                    buildingEntity.SetComponent(new HealthCurrent
                    {
                        Value = attackTower.Health.min,
                        GhostValue = attackTower.Health.min
                    });
                    buildingEntity.SetComponent(new ShootPoint
                    {
                        Value = ((AttackTowerView)view).ShootPoint
                    });
                    buildingEntity.AddComponent<AttackDamageRuntime>();
                    buildingEntity.SetComponent(new AttackDamage
                    {
                        Value = attackTower.Damage.min
                    });
                    buildingEntity.AddComponent<AttackRangeRuntime>();
                    buildingEntity.SetComponent(new AttackRange
                    {
                        Value = attackTower.AttackRange.min
                    });
                    buildingEntity.AddComponent<AttackCooldownRuntime>();
                    buildingEntity.SetComponent(new AttackCooldown
                    {
                        Value = attackTower.AttackCooldown.min
                    });
                    buildingEntity.SetComponent(new AttackProjectileData
                    {
                        EntityView = attackTower.ProjectileView,
                        ProjectileSpeed = 8f,
                    });
                    break;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}