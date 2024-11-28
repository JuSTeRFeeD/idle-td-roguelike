using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    //
    // Системы маркера над постройками, которые можно улучшить
    //
    
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class BuildingMarkSystem : ISystem
    {
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }
        
        private Filter _stoneStorageFilter;
        private Filter _woodStorageFilter;
        private Stash<WoodStorage> _woodStorageStash;
        private Stash<StoneStorage> _stoneStorageStash;

        private Filter _towersToUpgradeFilter;
        
        public void OnAwake()
        {
            _woodStorageFilter = World.Filter
                .With<WoodStorage>()
                .Build();
            _stoneStorageFilter = World.Filter
                .With<StoneStorage>()
                .Build();
            _woodStorageStash = World.GetStash<WoodStorage>();
            _stoneStorageStash = World.GetStash<StoneStorage>();
            
            _towersToUpgradeFilter = World.Filter
                .With<BuildingTag>()
                .Without<MaxLevelReachedTag>()
                .Without<BuildingWithUpgradeMark>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
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
            
            foreach (var tower in _towersToUpgradeFilter)
            {
                ref readonly var buildingTag = ref tower.GetComponent<BuildingTag>();
                if (_buildingsDatabase.TryGetById(buildingTag.BuildingConfigId, out var buildingConfig) &&
                    buildingConfig is UpgradableTowerConfig upgradableTowerConfig &&
                    buildingTag.Level < upgradableTowerConfig.UpgradePrices.Length)
                {
                    var price = upgradableTowerConfig.UpgradePrices[buildingTag.Level];
                    if (price.stonePrice > stoneAmount || price.woodPrice >= woodAmount) continue;

                    var mark = World.CreateEntity();
                    mark.AddComponent<UpgradeMarkTag>();
                    mark.InstantiateView(_worldSetup.WorldMarkView, tower.ViewPosition(), Quaternion.identity);
                    
                    tower.SetComponent(new BuildingWithUpgradeMark
                    {
                        MarkEntity = mark
                    });
                    
                    return;
                }
            }
        }

        public void Dispose()
        {
        }
    }
    
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class BuildingUnMarkSystem : ISystem
    {
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }
        
        private Filter _stoneStorageFilter;
        private Filter _woodStorageFilter;
        private Stash<WoodStorage> _woodStorageStash;
        private Stash<StoneStorage> _stoneStorageStash;

        private Filter _towersToUpgradeFilter;
        
        public void OnAwake()
        {
            _woodStorageFilter = World.Filter
                .With<WoodStorage>()
                .Build();
            _stoneStorageFilter = World.Filter
                .With<StoneStorage>()
                .Build();
            _woodStorageStash = World.GetStash<WoodStorage>();
            _stoneStorageStash = World.GetStash<StoneStorage>();
            
            _towersToUpgradeFilter = World.Filter
                .With<BuildingTag>()
                .With<BuildingWithUpgradeMark>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
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
            
            foreach (var tower in _towersToUpgradeFilter)
            {
                if (tower.Has<MaxLevelReachedTag>())
                {
                    ClearMark(tower);
                    continue;
                }
                
                ref readonly var buildingTag = ref tower.GetComponent<BuildingTag>();
                if (_buildingsDatabase.TryGetById(buildingTag.BuildingConfigId, out var buildingConfig) &&
                    buildingConfig is UpgradableTowerConfig upgradableTowerConfig &&
                    buildingTag.Level < upgradableTowerConfig.UpgradePrices.Length)
                {
                    var price = upgradableTowerConfig.UpgradePrices[buildingTag.Level];
                    if (price.stonePrice > stoneAmount || price.woodPrice >= woodAmount)
                    {
                        ClearMark(tower);
                    }
                    return;
                }
            }
        }

        private static void ClearMark(Entity tower)
        {
            tower.GetComponent<BuildingWithUpgradeMark>().MarkEntity.Dispose();
            tower.RemoveComponent<BuildingWithUpgradeMark>();
        }

        public void Dispose()
        {
        }
    }
}