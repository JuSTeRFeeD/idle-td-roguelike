using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Systems.FindTarget;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Projectile
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class PerformAoeAttackCastSystem : ISystem
    {
        public World World { get; set; }

        private Filter _aoeFilter;
        private Filter _aoeToGroundedFilter;
        private Filter _aoeToFlyFilter;
        
        private Filter _groundedEnemiesFilter;
        private Filter _flyEnemiesFilter;

        private Stash<PerformingDamage> _performingDamageStash;
        private Stash<AttackRangeRuntime> _attackRangeRuntimeStash;
        private Stash<ViewEntity> _viewEntityStash;

        private Entity[] _hits = new Entity[128];
        
        public void OnAwake()
        {
            _performingDamageStash = World.GetStash<PerformingDamage>();
            _attackRangeRuntimeStash = World.GetStash<AttackRangeRuntime>();
            _viewEntityStash = World.GetStash<ViewEntity>();
            
            _aoeFilter = World.Filter
                .With<AoeCastTag>()
                .With<ProjectileTag>()
                .With<PerformingDamage>()
                .With<AttackRangeRuntime>()
                .With<ViewEntity>()
                .Without<IsOnDelayToPerformAttack>()
                .Build();
            _aoeToGroundedFilter = World.Filter
                .With<AoeCastTag>()
                .With<ProjectileTag>()
                .With<PerformingDamage>()
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
                if (entity.Has<ShootVfx>())
                {
                    VfxPool.Spawn(entity.GetComponent<ShootVfx>().Value, _viewEntityStash.Get(entity).Value.RealModelPosition.position);
                }
                entity.Dispose();
            }
        }
        
        private void DealDamageInRange(Entity entity, in Filter filter)
        {
            var attackDamageRuntime= _performingDamageStash.Get(entity).Value;
            var count = FindTargetExtension.GetInRangeFilterNoAlloc(entity, filter, _attackRangeRuntimeStash.Get(entity).Value, _viewEntityStash, ref _hits);
            for (var i = 0; i < count; i++)
            {
                var hitTo = _hits[i];
                
                // Creating hit event
                var aoeHitEntity = World.CreateEntity();
                aoeHitEntity.AddComponent<ProjectileTag>();
                aoeHitEntity.SetComponent<PerformingDamage>(_performingDamageStash.Get(entity));
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