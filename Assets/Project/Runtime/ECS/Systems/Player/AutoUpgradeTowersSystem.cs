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
        [Inject] private ResourceCounter _resourceCounter;
        
        public World World { get; set; }
        
        private Filter _towersToUpgradeFilter;
        private float _delay;

        public void OnAwake()
        {
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
            
            foreach (var tower in _towersToUpgradeFilter)
            {
                ref readonly var buildingTag = ref tower.GetComponent<BuildingTag>();
                if (_buildingsDatabase.TryGetById(buildingTag.BuildingConfigId, out var buildingConfig) &&
                    buildingConfig is UpgradableTowerConfig upgradableTowerConfig &&
                    buildingTag.Level < upgradableTowerConfig.UpgradePrices.Length)
                {
                    var price = upgradableTowerConfig.UpgradePrices[buildingTag.Level];
                    if (price.stonePrice > _resourceCounter.StoneAmount ||
                        price.woodPrice >= _resourceCounter.WoodAmount)
                    {
                        continue;
                    }
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