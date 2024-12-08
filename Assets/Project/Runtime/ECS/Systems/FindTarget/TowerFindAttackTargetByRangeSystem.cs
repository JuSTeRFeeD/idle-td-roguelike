using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.FindTarget
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class TowerFindAttackTargetByRangeSystem : ISystem
    {
        public World World { get; set; }

        private Filter _attackBuildingsFilter;
        private Filter _enemiesFilter;
        
        private Stash<AttackRangeRuntime> _attackRangeRuntimeStash;
        private Stash<ViewEntity> _viewEntityStash;
        
        public void OnAwake()
        {
            _attackBuildingsFilter = World.Filter
                .With<BuildingTag>()
                .With<ViewEntity>()
                .With<AttackRangeRuntime>()
                .Without<AttackTarget>()
                .Without<ToDestroyTag>()
                .Without<DestroyedTag>()
                .Build();

            _enemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .Without<WillDeadAtNextTickTag>()
                .Build();
            
            _attackRangeRuntimeStash = World.GetStash<AttackRangeRuntime>();
            _viewEntityStash = World.GetStash<ViewEntity>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _attackBuildingsFilter)
            {
                FindTargetExtension.FindTargetWithFilter(entity, _enemiesFilter, _attackRangeRuntimeStash, _viewEntityStash);
            }
        }

        public void Dispose()
        {
        }
    }
}