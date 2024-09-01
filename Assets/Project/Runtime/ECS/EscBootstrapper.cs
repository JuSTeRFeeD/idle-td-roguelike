using System;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Systems;
using Project.Runtime.ECS.Systems.Building;
using Project.Runtime.ECS.Systems.GameCycle;
using Project.Runtime.ECS.Systems.Pathfinding;
using Project.Runtime.ECS.Systems.Player;
using Project.Runtime.ECS.Systems.Units;
using Project.Runtime.Features.Enemies;
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
            
            _commonSystemsGroup = world.CreateSystemsGroup();
            _postTickSystems = world.CreateSystemsGroup();
        }
        
        public void Initialize()
        {
            AddOneFrames();
            AddCommonSystems();
            AddPostTickSystems();
            
            Debug.Log("Esc Initialized!");
        }

        private void AddOneFrames()
        {
            _world.RegisterOneFrame<EntityClickEvent>();
            _world.RegisterOneFrame<MoveToTargetCompleted>();
        }

        private void AddCommonSystems()
        {
            _commonSystemsGroup.AddInitializer<DayNightInitializer>();
            _commonSystemsGroup.AddInitializer<PlayerDataInitializer>();
            _commonSystemsGroup.AddInitializer<ViewEntityDisposableInitializer>();
            
            _commonSystemsGroup.AddSystem<RuntimeStatsResetSystem>();
            
            // --- Building ---
            _commonSystemsGroup.AddInitializer<InitFirstBuildingsInitializer>();
            _commonSystemsGroup.AddInitializer<RandomResourcesSpawnInitializer>(); 
            // Building by user
            _commonSystemsGroup.AddSystem<StartPlacingBuildingSystem>();
            _commonSystemsGroup.AddSystem<PlacingBuildingSystem>();
            _commonSystemsGroup.AddSystem<PlaceBuildingSystem>();
            
            _commonSystemsGroup.AddSystem<CameraMoveSystem>();
            
            // --- Pathfinding ---
            _commonSystemsGroup.AddSystem<AStarPathfindingSystem>();
            _commonSystemsGroup.AddSystem<AStarMoveSystem>();
            
            // --- Units---
            _commonSystemsGroup.AddSystem<SpawnUnitSystem>();
            // Gathering
            _commonSystemsGroup.AddSystem<UnitFindResourceTargetSystem>();
            // _commonSystemsGroup.AddSystem<UnitMoveToTargetResourceSystem>(); // todo remove cuz rewrited with AStarMoveSystem
            _commonSystemsGroup.AddSystem<UnitGatheringResourceStartSystem>();
            _commonSystemsGroup.AddSystem<UnitGatheringResourceSystem>();
            // Move and put to storage
            _commonSystemsGroup.AddSystem<UnitFindStorageSystem>();
            // _commonSystemsGroup.AddSystem<UnitMoveToStorageSystem>(); // todo remove cuz rewrited with AStarMoveSystem
            _commonSystemsGroup.AddSystem<UnitPutResourcesToStorageSystem>();
            
            _commonSystemsGroup.AddSystem<FollowOwnerSystem>();
            
            _commonSystemsGroup.AddSystem<BuildingClickSystem>();

            _commonSystemsGroup.AddSystem<FindAttackTargetByRangeSystem>();
                
            // --- Enemies ---
            _commonSystemsGroup.AddSystem<NightTimeWaveSystem>();
            _commonSystemsGroup.AddSystem<SpawnEnemySystem>();
            
            _world.AddSystemsGroup(0, _commonSystemsGroup);
        }

        private void AddPostTickSystems()
        {
            _postTickSystems.AddSystem<AddExpMultiplierPerkSystem>();
            _postTickSystems.AddSystem<AddExpSystem>();

            _postTickSystems.AddSystem<LevelUpSystem>();
            
            _postTickSystems.AddSystem<TotalUnitsCountSystem>();
            _postTickSystems.AddSystem<TotalResourcesCountSystem>();
            
            _postTickSystems.AddSystem<DayNightSystem>();
                
            _world.AddSystemsGroup(4, _postTickSystems);
        }
        
        public void Dispose()
        {
            // _commonSystemsGroup?.Dispose();
            // _postTickSystems?.Dispose();

            // _world.RemoveSystemsGroup(_commonSystemsGroup);
            // _world.RemoveSystemsGroup(_postTickSystems);
            // world?.Dispose();
        }
    }
}