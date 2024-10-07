using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class UpgradeBuildingSystem : ISystem
    {
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private MapManager _mapManager;

        private Filter _stoneStorageFilter;
        private Filter _woodStorageFilter;
        private Stash<WoodStorage> _woodStorageStash;
        private Stash<StoneStorage> _stoneStorageStash;

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<BuildingTag>()
                .With<UpgradeBuildingRequest>()
                .Without<DestroyedTag>()
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

        public World World { get; set; }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                OnClickTowerUpgrade(entity);
            }
        }

        private void OnClickTowerUpgrade(Entity entity)
        {
            var buildingTag = entity.GetComponent<BuildingTag>();
            var buildingId = buildingTag.BuildingConfigId;
            if (!_buildingsDatabase.TryGetById(buildingId, out var config)) return;

            // Get prices
            if (config is not UpgradableTowerConfig upgradableTowerConfig) return;
            var prices = upgradableTowerConfig.UpgradePrices[buildingTag.Level];
            
            // Decreasing resources
            TakeResourcesFromStorages(prices);

            // Spawn building over other building to auto merge in building systems
            var request = World.CreateEntity();
            request.SetComponent(new PlacingBuildingCard
            {
                BuildingConfig = config,
                CurrentPosition = entity.ViewPosition(),
                IsCollisionDetected = true,
                IsMergeCollisionDetected = true
            });
            request.SetComponent(new PlaceBuildingCardRequest());
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

        public void Dispose()
        {
        }
    }
}