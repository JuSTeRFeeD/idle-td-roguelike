using System;
using DG.Tweening;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Views;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Features.TimeManagement;
using Project.Runtime.Scriptable.Buildings;
using Project.Runtime.Scriptable.Enemies;
using Project.Runtime.Services.PlayerProgress;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class PlaceBuildingSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private VfxSetup _vfxSetup;
        [Inject] private MapManager _mapManager;
        [Inject] private CameraController _cameraController;
        [Inject] private HandsManager _handsManager;
        [Inject] private InventoryStorage _inventoryStorage;
        [Inject] private PersistentPlayerData _persistentPlayerData;
        
        public World World { get; set; }

        private Filter _filter;
        private Filter _statisticsFilter;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<PlaceBuildingCardRequest>()
                .With<PlacingBuildingCard>()
                .Build();
            _statisticsFilter = World.Filter
                .With<TotalPlacedTowersStatistic>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                _handsManager.SetPlacingEnabledEnabled(false);
                _handsManager.SetIsCardDrag(false);
                
                TimeScale.SetNormalTimeScale();

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
                        placingBuilding.CellEntity?.Dispose();
                        entity.Dispose();
                        return;
                    }

                    var building = _mapManager.UpgradeBuilding(towerConfig, gridPos);
                    if (building == null)
                    {
                        Debug.LogError("Нужно обработать если building null");
                        placingBuilding.CellEntity?.Dispose();
                        entity.Dispose();
                        return;
                    }
                    
                    var mergedToBuildingTag = building.Entity.GetComponent<BuildingTag>();
                    building.Entity.AddComponent<BuildingUpgraded>();
                    
                    // Upgrade view visual level
                    view = building.Entity.GetComponent<ViewEntity>().Value;
                    var towerView = (AttackTowerView)view; 
                    if (towerView.TowerViewUpgrades) towerView.TowerViewUpgrades.SetLevel(mergedToBuildingTag.Level);
                    
                    // Upgrade stats
                    UpgradeBuildingEntity(building.Entity, towerConfig, mergedToBuildingTag.Level);
                
                    VfxPool.Spawn(_vfxSetup.TowerLevelUpVfx, towerView.transform.position);
                }
                // Put new
                else 
                {
                    var buildingEntity = World.CreateEntity();
                    
                    // for systems know about map changes
                    var changeEvent = World.CreateEntity();
                    changeEvent.AddComponent<MapGridChangedOneFrame>();
                    changeEvent.AddComponent<GridPlacedTowerOneFrame>();
                    
                    view = _mapManager.PutBuilding(
                        placingBuilding.BuildingConfig, 
                        gridPos,
                        Quaternion.identity, 
                        buildingEntity);
                    buildingEntity.LinkView(view);

                    if (view is AttackTowerView towerView && towerView.TowerViewUpgrades)
                    {
                        towerView.TowerViewUpgrades.SetLevel(0);
                    }
                        
                    SetupEntity(buildingEntity, placingBuilding.BuildingConfig, placingBuilding.CardConfigId, view);

                    if (!isSystemAction)
                    {
                        _statisticsFilter.First().GetComponent<TotalPlacedTowersStatistic>().Value++;
                        VfxPool.Spawn(_vfxSetup.PutTowerVfx, view.transform.position);
                    }
                }

                if (!isSystemAction)
                {
                    view.transform.DOKill(true);
                    view.transform
                        .DOPunchScale(Vector3.up * .3f, 0.25f, 10, 2f)
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
                
                if (entity.Has<RadiusViewEntity>())
                {
                    entity.GetComponent<RadiusViewEntity>().Entity.Dispose();
                }
                placingBuilding.CellEntity?.Dispose();
                entity.Dispose();
            }
        }

        private void UpgradeBuildingEntity(Entity buildingEntity, UpgradableTowerConfig buildingConfig, int level)
        {
            var maxLevel = buildingConfig.UpgradePrices.Length;
            
            // HP
            ref var currentHealth = ref buildingEntity.GetComponent<HealthCurrent>().Value;
            ref var defaultHealth = ref buildingEntity.GetComponent<HealthDefault>().Value;
            var prevHealthPercent = currentHealth / defaultHealth;
            defaultHealth = buildingConfig.Health.Evaluate(level, maxLevel);
            currentHealth = defaultHealth * prevHealthPercent;
            
            switch (buildingConfig)
            {
                case AttackTowerBuildingConfig attackTower:
                {
                    // Attack stats
                    buildingEntity.GetComponent<AttackDamage>().Value = attackTower.Damage.Evaluate(level, maxLevel);
                    buildingEntity.GetComponent<AttackCooldown>().Value = attackTower.AttackCooldown.Evaluate(level, maxLevel);
                    buildingEntity.GetComponent<AttackRange>().Value = attackTower.AttackRange.Evaluate(level, maxLevel);
                    break;
                }
            }
        }

        private void SetupEntity(in Entity buildingEntity, in BuildingConfig buildingConfig,
            string placingBuildingCardConfigId, in EntityView view)
        {
            var cardSaveData = _persistentPlayerData.GetCardSaveDataByCardId(placingBuildingCardConfigId);
            var addHealthPercentByLvl = 0f;
            var addDamagePercentByLvl = 0f;
            if (cardSaveData != null)
            {
                addHealthPercentByLvl = 0.1f * cardSaveData.level;
                addDamagePercentByLvl = 0.15f * cardSaveData.level;
            }

            // Link Animator
            if (view is AttackTowerView attackTowerView && attackTowerView.Animator)
            {
                buildingEntity.SetComponent(new ViewAnimator
                {
                    Value = attackTowerView.Animator
                });
            }
            
            switch (buildingConfig)
            {
                case BaseBuildingConfig baseBuilding:
                {
                    buildingEntity.SetComponent(new BuildingTag
                    {
                        BuildingConfigId = buildingConfig.uniqueID,
                        Size = buildingConfig.Size,
                    });
                    buildingEntity.AddComponent<BaseTowerTag>();

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
                    buildingEntity.AddComponent<MapResourceTag>();
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
                            buildingEntity.AddComponent<TreeTag>();
                            break;
                        case ResourceType.Stone:
                            buildingEntity.AddComponent<StoneTag>();
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
                        Size = buildingConfig.Size,
                        Level = 0,
                        MaxLevel = attackTower.UpgradePrices.Length
                    });
                    switch (attackTower.AttackTowerType)
                    {
                        case AttackTowerType.Cannon:
                            buildingEntity.AddComponent<CannonTowerTag>();
                            buildingEntity.AddComponent<SplashDamageRuntime>();
                            buildingEntity.SetComponent(new SplashDamage
                            {
                                Radius = 2f,
                                PercentFromDamage = 0.1f
                            });
                            break;
                        
                        case AttackTowerType.Crossbow:
                            buildingEntity.AddComponent<CrossbowTowerTag>();
                            break;
                        
                        case AttackTowerType.Crystal:
                            buildingEntity.AddComponent<CrystalTowerTag>();
                            buildingEntity.AddComponent<TowerWithBouncingProjectileRuntime>();
                            buildingEntity.SetComponent(new TowerWithBouncingProjectile
                            {
                                Bounces = 1
                            });
                            break;
                        
                        case AttackTowerType.Bomb:
                            buildingEntity.AddComponent<BombTowerTag>();
                            buildingEntity.AddComponent<OneLifeTag>(); 
                            break;
                        
                        case AttackTowerType.Tomb:
                            buildingEntity.AddComponent<TombTowerTag>();
                            buildingEntity.AddComponent<AoeAttackingTowerTag>();
                            buildingEntity.AddComponent<TowerAttackLandsToTheGroundEnemy>(); // TODO: это в прокачку тавера вынести
                            buildingEntity.SetComponent(new DelayToPerformAttack
                            {
                                Value = 0.1f
                            });
                            break;
                        case AttackTowerType.Snowman:
                            buildingEntity.AddComponent<SnowmanTowerTag>();
                            break;
                        case AttackTowerType.Pumpkin:
                            buildingEntity.AddComponent<PumpkinTowerTag>();
                            if (!attackTower.HitVfx) Debug.LogError("Pumpkin doesn't have hit vfx for poison!");
                            buildingEntity.SetComponent(new SpawnPoisonDustOnHit
                            {
                                PoisonVFX = attackTower.HitVfx,
                                Damage = 50,
                                Lifetime = 3.01f,
                                TimeBetweenAttack = 0.25f
                            });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (var attackTowerFocusEnemyType in attackTower.FocusEnemyTypes)
                    {
                        Debug.Log("TOWER FOCUSING " + attackTowerFocusEnemyType);
                        switch (attackTowerFocusEnemyType)
                        {
                            case EnemyType.Ground:
                                buildingEntity.AddComponent<FocusGroundEnemiesTag>();
                                break;
                            case EnemyType.Flying:
                                buildingEntity.AddComponent<FocusFlyingEnemiesTag>();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }   
                    }
                    
                    // Health
                    var initHp = attackTower.Health.min + addHealthPercentByLvl * attackTower.Health.min;
                    buildingEntity.SetComponent(new HealthDefault
                    {
                        Value = initHp
                    });
                    buildingEntity.SetComponent(new HealthCurrent
                    {
                        Value = initHp,
                        GhostValue = initHp
                    });
                    
                    // Critical chance & damage
                    buildingEntity.AddComponent<CriticalChanceRuntime>();
                    buildingEntity.SetComponent(new CriticalChance
                    {
                        Value = 0.5f
                    });
                    buildingEntity.AddComponent<CriticalDamageRuntime>();
                    buildingEntity.SetComponent(new CriticalDamage
                    {
                        Value = 1f
                    });
                    
                    // Attack
                    var initDmg = attackTower.Damage.min + addDamagePercentByLvl * attackTower.Damage.min;
                    buildingEntity.AddComponent<AttackDamageRuntime>();
                    buildingEntity.SetComponent(new AttackDamage
                    {
                        Value = initDmg
                    });
                    
                    // Range
                    buildingEntity.AddComponent<AttackRangeRuntime>();
                    buildingEntity.SetComponent(new AttackRange
                    {
                        Value = attackTower.AttackRange.min
                    });
                    
                    // Cooldown
                    buildingEntity.AddComponent<AttackCooldownRuntime>();
                    buildingEntity.SetComponent(new AttackCooldown
                    {
                        Value = attackTower.AttackCooldown.min
                    });
                    
                    // Projectile & shooting
                    if (attackTower.ProjectileView)
                    {
                        buildingEntity.SetComponent(new ProjectileData
                        {
                            EntityView = attackTower.ProjectileView,
                            ProjectileSpeed = attackTower.ProjectileSpeed,
                        });
                        buildingEntity.SetComponent(new ShootPoint
                        {
                            Value = ((AttackTowerView)view).ShootPoint
                        });
                    }
                    if (attackTower.ShootVfx)
                    {
                        buildingEntity.SetComponent(new ShootVfx
                        {
                            Value = attackTower.ShootVfx
                        });
                    }
                    if (attackTower.HitVfx)
                    {
                        buildingEntity.SetComponent(new HitVfx
                        {
                            Value = attackTower.HitVfx
                        });
                    }
                    
                    break;
                }

                case SupportBuildingConfig supportBuildingConfig:
                {
                    buildingEntity.SetComponent(new BuildingTag
                    {
                        BuildingConfigId = buildingConfig.uniqueID,
                        Size = buildingConfig.Size,
                        Level = 0,
                        MaxLevel = supportBuildingConfig.UpgradePrices.Length
                    });
                    
                    // Health
                    buildingEntity.SetComponent(new HealthDefault
                    {
                        Value = supportBuildingConfig.Health.min
                    });
                    buildingEntity.SetComponent(new HealthCurrent
                    {
                        Value = supportBuildingConfig.Health.min,
                        GhostValue = supportBuildingConfig.Health.min
                    });
                    
                    if (supportBuildingConfig.IsOneLifeBuilding)
                    {
                        buildingEntity.AddComponent<OneLifeTag>();
                    }

                    switch (supportBuildingConfig.SupportType)
                    {
                        case SupportBuildingConfig.SupportTowerType.Dummy:
                            buildingEntity.AddComponent<DummyTowerTag>();
                            buildingEntity.SetComponent(new ReturnOfReceivedDamage
                            {
                                Percent = 2f,
                                FixedReturnDamage = 10f,
                            });
                            break;
                        case SupportBuildingConfig.SupportTowerType.Candy:
                            buildingEntity.AddComponent<CandyTowerTag>();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    break;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}