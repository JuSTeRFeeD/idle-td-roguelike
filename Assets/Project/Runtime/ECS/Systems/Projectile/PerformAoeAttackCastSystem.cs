using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Systems.FindTarget;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Projectile
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class PerformAoeAttackCastSystem : ISystem
    {
        [Inject] private VfxSetup _vfxSetup;
        
        public World World { get; set; }

        private Filter _aoeFilter;
        private Filter _aoeToGroundedFilter;
        private Filter _aoeToFlyFilter;
        
        private Filter _groundedEnemiesFilter;
        private Filter _flyEnemiesFilter;

        private Stash<AttackDamageRuntime> _attackDamageRuntimeStash;
        private Stash<AttackRangeRuntime> _attackRangeRuntimeStash;
        private Stash<ViewEntity> _viewEntityStash;

        private Entity[] _hits = new Entity[128];
        
        public void OnAwake()
        {
            _attackDamageRuntimeStash = World.GetStash<AttackDamageRuntime>();
            _attackRangeRuntimeStash = World.GetStash<AttackRangeRuntime>();
            _viewEntityStash = World.GetStash<ViewEntity>();
            
            _aoeFilter = World.Filter
                .With<AoeCastTag>()
                .With<ProjectileTag>()
                .With<AttackDamageRuntime>()
                .With<AttackRangeRuntime>()
                .With<ViewEntity>()
                .Without<IsOnDelayToPerformAttack>()
                .Build();
            _aoeToGroundedFilter = World.Filter
                .With<AoeCastTag>()
                .With<ProjectileTag>()
                .With<AttackDamageRuntime>()
                .With<AttackRangeRuntime>()
                .With<ViewEntity>()
                .With<FocusGroundEnemiesTag>()
                .Without<IsOnDelayToPerformAttack>()
                .Build();
            _aoeToFlyFilter = World.Filter
                .With<AoeCastTag>()
                .With<ProjectileTag>()
                .With<AttackDamageRuntime>()
                .With<AttackRangeRuntime>()
                .With<ViewEntity>()
                .With<FocusFlyingEnemiesTag>()
                .Without<IsOnDelayToPerformAttack>()
                .Build();
            
            _groundedEnemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .With<GroundEnemyTag>()
                .Without<WillDeadAtNextTickTag>()
                .Build();
            
            _flyEnemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .With<FlyingEnemyTag>()
                .Without<WillDeadAtNextTickTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // Focus Ground
            foreach (var entity in _aoeToGroundedFilter)
            {
                DealDamageInRange(entity, _groundedEnemiesFilter);
            }
            
            // Focus Fly
            foreach (var entity in _aoeToFlyFilter)
            {
                DealDamageInRange(entity, _flyEnemiesFilter);
            }
            
            foreach (var entity in _aoeFilter)
            {
                VfxPool.Spawn(_vfxSetup.PutTowerVfx, _viewEntityStash.Get(entity).Value.transform.position);
                entity.Dispose();
            }
        }
        
        private void DealDamageInRange(Entity entity, in Filter filter)
        {
            var damage = _attackDamageRuntimeStash.Get(entity).Value;
            var count = FindByAttackRangeExt.GetInRangeFilterNoAlloc(entity, filter, _attackRangeRuntimeStash.Get(entity).Value, _viewEntityStash, ref _hits);
            for (var i = 0; i < count; i++)
            {
                var hitTo = _hits[i];
                
                // creating hit event
                var aoeHitEntity = World.CreateEntity();
                aoeHitEntity.AddComponent<ProjectileTag>();
                aoeHitEntity.SetComponent(new AttackDamageRuntime
                {
                    Value = damage
                });
                aoeHitEntity.SetComponent(new ProjectileHit
                {
                    HitEntity = hitTo
                });
            }
        }

        public void Dispose()
        {
        }
    }
}