using System;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Systems;
using Project.Runtime.ECS.Systems.Attack;
using Project.Runtime.ECS.Systems.Building;
using Project.Runtime.ECS.Systems.Cooldown;
using Project.Runtime.ECS.Systems.Enemies;
using Project.Runtime.ECS.Systems.FindTarget;
using Project.Runtime.ECS.Systems.GameCycle;
using Project.Runtime.ECS.Systems.Pathfinding;
using Project.Runtime.ECS.Systems.Player;
using Project.Runtime.ECS.Systems.Projectile;
using Project.Runtime.ECS.Systems.Stats;
using Project.Runtime.ECS.Systems.TakingDamage;
using Project.Runtime.ECS.Systems.Units;
using Project.Runtime.ECS.Systems.Units.RepairBuildings;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Helpers.OneFrame;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Runtime.ECS
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class EscBootstrapper : IInitializable, IDisposable
    {
        private readonly World _world;
        
        private readonly SystemsGroup _commonSystemsGroup;
        private readonly SystemsGroup _postTickSystems;
        
        public EscBootstrapper(IObjectResolver container, World world)
        {
            SystemGroupExtension.SetResolver(container);
            
            _world = world;
            _world.UpdateByUnity = true;
            
            _commonSystemsGroup = world.CreateSystemsGroup();
            _postTickSystems = world.CreateSystemsGroup();
        }
        
        public void Initialize()
        {
            AddOneFrames();
            AddCommonSystems();
            AddPostTickSystems();
            Debug.Log($"Ecs Init World {_world.GetFriendlyName()}");
        }

        private void AddOneFrames()
        {
            _world.RegisterOneFrame<EntityClickEvent>();
            _world.RegisterOneFrame<UpgradeBuildingRequest>();
            _world.RegisterOneFrame<BuildingUpgraded>();
            _world.RegisterOneFrame<MoveToTargetCompleted>();
            _world.RegisterOneFrame<MapGridChangedOneFrame>();
            _world.RegisterOneFrame<AttackCooldownEndOneFrame>();
        }

        private void AddCommonSystems()
        {
#if UNITY_EDITOR
            _commonSystemsGroup.AddSystem<SystemsDev.FastFinishGameSystem>();
#endif
            
            _commonSystemsGroup.AddInitializer<DayNightInitializer>();
            _commonSystemsGroup.AddInitializer<PlayerDataInitializer>();
            _commonSystemsGroup.AddInitializer<ViewEntityDisposableInitializer>();
            
            // --- STATS ---
            _commonSystemsGroup.AddSystem<RuntimeStatsResetSystem>();
            _commonSystemsGroup.AddSystem<ApplyTowerPerkUpgradesSystem>();
            
            // --- Building ---
            _commonSystemsGroup.AddInitializer<SpawnFirstBuildingsInitializer>();
            // Building by user
            _commonSystemsGroup.AddSystem<StartPlacingBuildingSystem>();
            _commonSystemsGroup.AddSystem<PlacingBuildingSystem>();
            _commonSystemsGroup.AddSystem<PlaceBuildingSystem>();
            
            // --- Pathfinding ---
            _commonSystemsGroup.AddSystem<AStarPathfindingSystem>();
            _commonSystemsGroup.AddSystem<AStarMoveSystem>();
            
            // --- Units---
            _commonSystemsGroup.AddSystem<SpawnUnitSystem>();
            
            // Repair towers
            _commonSystemsGroup.AddSystem<FindUnitsToRepairTowersSystem>();
            _commonSystemsGroup.AddSystem<UnitRepairTowerSystem>();
            
            // Gathering
            _commonSystemsGroup.AddSystem<UnitSetFindResWhenNeedSystem>();
            _commonSystemsGroup.AddSystem<UnitFindResourceTargetSystem>();
            _commonSystemsGroup.AddSystem<UnitGatheringResourceStartSystem>();
            _commonSystemsGroup.AddSystem<UnitGatheringResourceSystem>();
            // Move and put to storage
            _commonSystemsGroup.AddSystem<UnitFindStorageSystem>();
            _commonSystemsGroup.AddSystem<UnitPutResourcesToStorageSystem>();
            
            _commonSystemsGroup.AddSystem<FollowOwnerSystem>();
            
            _commonSystemsGroup.AddSystem<BuildingClickSystem>();
            _commonSystemsGroup.AddSystem<AutoUpgradeTowersSystem>();
            _commonSystemsGroup.AddSystem<UpgradeBuildingSystem>();

            _commonSystemsGroup.AddSystem<EnemyFindAttackTargetByRangeSystem>();
            _commonSystemsGroup.AddSystem<TowerFindAttackTargetByRangeSystem>();
            _commonSystemsGroup.AddSystem<RotateTowerToAttackTargetSystem>();
                
            // --- Enemies ---
            _commonSystemsGroup.AddSystem<NightTimeWaveSystem>();
            _commonSystemsGroup.AddSystem<SpawnEnemySystem>();
            _commonSystemsGroup.AddSystem<EnemyFindMoveTargetSystem>();
            _commonSystemsGroup.AddSystem<EnemyMeleeAttackSystem>();
            _commonSystemsGroup.AddSystem<EnemyRefreshPathOnMapChangeSystem>();
            
            // --- Combat ---
            _postTickSystems.AddSystem<AttackCooldownSystem>();
            _postTickSystems.AddSystem<RemoveBombDestroyedOnCooldownEndSystem>();
            
            _commonSystemsGroup.AddSystem<UpdateDelayToPerformAttackSystem>();
            _commonSystemsGroup.AddSystem<AoeAttackStartSystem>();
            _commonSystemsGroup.AddSystem<ShootToAttackTargetSystem>();
            
            _commonSystemsGroup.AddSystem<TrajectoryProjectileMoveSystem>();
            _commonSystemsGroup.AddSystem<BounceProjectileSystem>();
            _commonSystemsGroup.AddSystem<SplashDamageSystem>();
            _commonSystemsGroup.AddSystem<PerformAoeAttackCastSystem>();
            _commonSystemsGroup.AddSystem<ProjectileHitLandToTheGroundEnemySystem>();
            _commonSystemsGroup.AddSystem<ProjectileHitDealDamageSystem>();
            
            // Taking damage
            _commonSystemsGroup.AddSystem<MarkBuildingAsDamagedSystem>();
            _commonSystemsGroup.AddSystem<ReturnOfReceivedDamageSystem>();
            _commonSystemsGroup.AddSystem<BombExplosionOnTakeDamageSystem>();
            _commonSystemsGroup.AddSystem<BaseTowerApplyDamageSystem>(); // will skip next system
            _commonSystemsGroup.AddSystem<DamagePopupSystem>();
            _commonSystemsGroup.AddSystem<ApplyDamageStatisticSystem>();
            _commonSystemsGroup.AddSystem<ApplyDamageSystem>();
            _commonSystemsGroup.AddSystem<DeathSystem>();
            
            
            _commonSystemsGroup.AddSystem<DestroyOverTimeSystem>();
            
            _world.AddSystemsGroup(0, _commonSystemsGroup);
        }

        private void AddPostTickSystems()
        {
            _postTickSystems.AddSystem<AddExpMultiplierPerkSystem>();
            _postTickSystems.AddSystem<AddExpSystem>();
            
            _postTickSystems.AddSystem<CameraMoveSystem>();
            
            _postTickSystems.AddSystem<LevelUpSystem>();
            
            _postTickSystems.AddSystem<TotalUnitsCountSystem>();
            _postTickSystems.AddSystem<TotalResourcesCountSystem>();
            
            _postTickSystems.AddSystem<DayNightSystem>();
            
            _postTickSystems.AddSystem<GameOverWhenBaseDestroyedSystem>();
            _postTickSystems.AddSystem<GameFinishedSystem>();
                
            _world.AddSystemsGroup(4, _postTickSystems);
        }
        
        public void Dispose()
        {
            _world?.Dispose();
        }
    }
}