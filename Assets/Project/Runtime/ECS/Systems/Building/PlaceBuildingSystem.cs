using System;
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
        [Inject] private MapManager _mapManager;
        [Inject] private CameraController _cameraController;
        [Inject] private HandsManager _handsManager;
        
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
                
                var placingBuilding = entity.GetComponent<PlacingBuildingCard>();
                
                var view = _mapManager.PutBuilding(
                    placingBuilding.BuildingConfig, 
                    GridUtils.ConvertWorldToGridPos(placingBuilding.CurrentPosition),
                    Quaternion.identity);
                var buildingEntity = World.CreateEntity();
                buildingEntity.LinkView(view);

                SetupEntity(buildingEntity, placingBuilding.BuildingConfig, view);
                
                _cameraController.ResetTarget();

                placingBuilding.CellEntity?.Dispose();
                entity.Dispose();
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
                        Size = buildingConfig.Size
                    });
                    buildingEntity.SetComponent(new BaseTowerTag());

                    buildingEntity.SetComponent(new HealthDefault
                    {
                        Value = baseBuilding.Health
                    });
                    buildingEntity.SetComponent(new HealthCurrent
                    {
                        Value = baseBuilding.Health
                    });
                    buildingEntity.SetComponent(new LumberjackUnitsOwnedTag
                    {
                        CurrentCapacity = baseBuilding.LumberjacksCapacity.min,
                        Capacity = baseBuilding.LumberjacksCapacity,
                    });
                    buildingEntity.SetComponent(new WoodStorage
                    {
                        Current = 0,
                        Max = baseBuilding.WoodStorageSize
                    });
                    buildingEntity.SetComponent(new MinerUnitsOwnedTag
                    {
                        CurrentCapacity = baseBuilding.MinerCapacity.min,
                        Capacity = baseBuilding.MinerCapacity
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
                            UnitType = UnitType.Lumberjack
                        });
                    }

                    break;
                }

                case MapResourceConfig resource: 
                {
                    buildingEntity.SetComponent(new HealthDefault
                    {
                        Value = resource.health
                    });
                    buildingEntity.SetComponent(new HealthCurrent
                    {
                        Value = resource.health
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
                        Size = buildingConfig.Size
                    });
                    buildingEntity.SetComponent(new AttackTowerTag());

                    buildingEntity.SetComponent(new HealthDefault
                    {
                        Value = attackTower.Health
                    });
                    buildingEntity.SetComponent(new HealthCurrent
                    {
                        Value = attackTower.Health
                    });
                    buildingEntity.SetComponent(new ShootPoint
                    {
                        Value = ((AttackTowerView)view).ShootPoint
                    });
                    buildingEntity.AddComponent<AttackDamageRuntime>();
                    buildingEntity.SetComponent(new AttackDamage
                    {
                        Value = attackTower.Damage
                    });
                    buildingEntity.AddComponent<AttackRangeRuntime>();
                    buildingEntity.SetComponent(new AttackRange
                    {
                        Value = attackTower.AttackRange
                    });
                    buildingEntity.AddComponent<AttackCooldownRuntime>();
                    buildingEntity.SetComponent(new AttackCooldown
                    {
                        Value = attackTower.AttackCooldown
                    });
                    buildingEntity.SetComponent(new ProjectileParams
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