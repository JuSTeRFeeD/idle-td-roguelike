using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.FindTarget
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class EnemyFindAttackTargetByRangeSystem : ISystem
    {
        public World World { get; set; }

        private Filter _enemiesFilter;
        
        private Filter _allyBuildingsFilter;
        
        private Stash<AttackRangeRuntime> _attackRangeRuntimeStash;
        private Stash<ViewEntity> _viewEntityStash;
        
        public void OnAwake()
        {
            _enemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .With<AttackRangeRuntime>()
                .Without<AttackTarget>()
                .Without<WillDeadAtNextTickTag>()
                .Build();
            
            _allyBuildingsFilter = World.Filter
                .With<BuildingTag>()
                .With<ViewEntity>()
                .Without<DestroyedTag>()
                .Without<ToDestroyTag>()
                .Build();

            _attackRangeRuntimeStash = World.GetStash<AttackRangeRuntime>();
            _viewEntityStash = World.GetStash<ViewEntity>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _enemiesFilter)
            {
                FindByAttackRange.FindTargetWithFilter(entity, _allyBuildingsFilter, _attackRangeRuntimeStash, _viewEntityStash);
            }
        }

        public void Dispose()
        {
        }
    }
}