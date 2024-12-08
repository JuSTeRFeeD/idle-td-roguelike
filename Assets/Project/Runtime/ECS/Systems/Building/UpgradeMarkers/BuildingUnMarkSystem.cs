using Project.Runtime.ECS.Components;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class BuildingUnMarkSystem : ISystem
    {
        [Inject] private BuildingsDatabase _buildingsDatabase;
        [Inject] private WorldSetup _worldSetup;
        [Inject] private ResourceCounter _resourceCounter;

        public World World { get; set; }

        private Filter _towersToUpgradeFilter;
        private Filter _markFilter;

        public void OnAwake()
        {
            _towersToUpgradeFilter = World.Filter
                .With<BuildingTag>()
                .With<BuildingWithUpgradeMark>()
                .Build();
            _markFilter = World.Filter.With<UpgradeMarkTag>().Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _markFilter)
            {
                ref readonly var mark = ref entity.GetComponent<UpgradeMarkTag>();
                if (mark.Building.IsNullOrDisposed())
                {
                    entity.Dispose();
                    continue;
                }

                if (mark.Building.Has<DestroyedTag>())
                {
                    if (mark.Building.Has<BuildingWithUpgradeMark>())
                    {
                        mark.Building.RemoveComponent<BuildingWithUpgradeMark>();
                    }
                    entity.Dispose();
                }
            }

            World.Commit();


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
                    if (price.stonePrice > _resourceCounter.StoneAmount || price.woodPrice >= _resourceCounter.WoodAmount ||
                        buildingTag.Level >= upgradableTowerConfig.UpgradePrices.Length)
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