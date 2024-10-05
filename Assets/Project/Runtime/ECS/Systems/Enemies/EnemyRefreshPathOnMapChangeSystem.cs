using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Enemies
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class EnemyRefreshPathOnMapChangeSystem : ISystem
    {
        public World World { get; set; }

        private Filter _mapChangedFilter;
        private Filter _enemiesFilter;
        
        public void OnAwake()
        {
            _mapChangedFilter = World.Filter
                .With<MapGridChangedOneFrame>()
                .With<GridPlacedTowerOneFrame>()
                .Build();
            _enemiesFilter = World.Filter
                .With<EnemyTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_mapChangedFilter.IsEmpty()) return;

            foreach (var entity in _enemiesFilter)
            {
                UnityEngine.Debug.Log("Refresh path for unit");
                entity.SafeRemove<AStarPath>();
                entity.SafeRemove<AttackTarget>();
                entity.SafeRemove<AStarCalculatePathRequest>();
            }
        }

        public void Dispose()
        {
        }
    }
}