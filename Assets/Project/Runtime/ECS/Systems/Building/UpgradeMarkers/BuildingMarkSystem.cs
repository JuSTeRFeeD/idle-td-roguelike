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
        [Inject] private ResourceCounter _resourceCounter;

        public World World { get; set; }
        
        private Filter _towersToUpgradeFilter;

        public void OnAwake()
        {
            _towersToUpgradeFilter = World.Filter
                .With<BuildingTag>()
                .Without<IsAttackOnCooldown>()
                .Without<MaxLevelReachedTag>()
                .Without<BuildingWithUpgradeMark>()
                .Without<DestroyedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
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

                    var mark = World.CreateEntity();
                    mark.SetComponent(new UpgradeMarkTag { Building = tower });
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
}