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

        private Filter _buildingsGroundFocusFilter;
        private Filter _buildingsFlyingFocusFilter;
        
        private Filter _groundEnemiesFilter;
        private Filter _flyingEnemiesFilter;
        
        private Stash<AttackRangeRuntime> _attackRangeRuntimeStash;
        private Stash<ViewEntity> _viewEntityStash;
        
        public void OnAwake()
        {
            _buildingsGroundFocusFilter = World.Filter
                .With<BuildingTag>()
                .With<ViewEntity>()
                .With<AttackRangeRuntime>()
                .With<TowerFocusGroundEnemiesTag>()
                .Without<AttackTarget>()
                .Without<ToDestroyTag>()
                .Without<DestroyedTag>()
                .Build();
            _buildingsFlyingFocusFilter = World.Filter
                .With<BuildingTag>()
                .With<ViewEntity>()
                .With<AttackRangeRuntime>()
                .With<TowerFocusFlyingEnemiesTag>()
                .Without<AttackTarget>()
                .Without<ToDestroyTag>()
                .Without<DestroyedTag>()
                .Build();

            _groundEnemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .With<GroundEnemyTag>()
                .Without<WillDeadAtNextTickTag>()
                .Build();
            _flyingEnemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .With<FlyingEnemyTag>()
                .Without<WillDeadAtNextTickTag>()
                .Build();
            
            _attackRangeRuntimeStash = World.GetStash<AttackRangeRuntime>();
            _viewEntityStash = World.GetStash<ViewEntity>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _buildingsFlyingFocusFilter)
            {
                FindByAttackRange.FindTargetWithFilter(entity, _flyingEnemiesFilter, _attackRangeRuntimeStash, _viewEntityStash);
            }
            World.Commit();
            foreach (var entity in _buildingsGroundFocusFilter)
            {
                FindByAttackRange.FindTargetWithFilter(entity, _groundEnemiesFilter, _attackRangeRuntimeStash, _viewEntityStash);
            }
        }

        public void Dispose()
        {
        }
    }
}