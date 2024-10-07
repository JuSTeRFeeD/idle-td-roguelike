using Project.Runtime.ECS.Components;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Buildings;
using Project.Runtime.Services.PlayerProgress;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class AutoUpgradeTowersSystem : ISystem
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private BuildingsDatabase _buildingsDatabase;
        
        public World World { get; set; }

        private Filter _stoneStorageFilter;
        private Filter _woodStorageFilter;
        private Stash<WoodStorage> _woodStorageStash;
        private Stash<StoneStorage> _stoneStorageStash;
        
        private Filter _towersToUpgradeFilter;
        private float _delay;

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
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_persistentPlayerData.AutoUpgradeTowersChecked) return;

            _delay -= deltaTime;
            if (_delay > 0) return;
            _delay = 0.5f;
            
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
                if (_buildingsDatabase.TryGetById(buildingTag.BuildingConfigId, out var buildingConfig)
                    && buildingConfig is UpgradableTowerConfig upgradableTowerConfig)
                {
                    var price = upgradableTowerConfig.UpgradePrices[buildingTag.Level];
                    if (price.stonePrice > stoneAmount || price.woodPrice >= woodAmount) continue;
                    tower.AddComponent<UpgradeBuildingRequest>();
                    return;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}