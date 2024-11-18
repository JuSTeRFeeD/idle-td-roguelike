using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Attack
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class AoeTowerAttackStartSystem : ISystem
    {
        [Inject] private VfxSetup _vfxSetup;
        
        public World World { get; set; }

        private Filter _filter;
        private static readonly int AnimAttackTrigger = Animator.StringToHash("attack");

        public void OnAwake()
        {
            _filter = World.Filter
                .With<AttackTarget>()
                .With<ShootPoint>()
                .With<AttackDamageRuntime>()
                .With<AttackCooldownRuntime>()
                .With<AttackRangeRuntime>()
                .With<AoeAttackingTowerTag>()
                .Without<IsAttackOnCooldown>()
                .Without<IsOnDelayToPerformAttack>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var attackTarget = ref entity.GetComponent<AttackTarget>().Value;
                
                if (attackTarget.IsNullOrDisposed())
                {
                    entity.RemoveComponent<AttackTarget>();
                    continue;
                }
                
                ref readonly var attackRangeRuntime = ref entity.GetComponent<AttackRangeRuntime>().Value;
                
                // Now this entity out of range
                if (Vector3.SqrMagnitude(attackTarget.ViewPosition() - entity.ViewPosition()) >
                    attackRangeRuntime * attackRangeRuntime)
                {
                    entity.RemoveComponent<AttackTarget>();
                    continue;
                }
                
                ref readonly var projectileParams = ref entity.GetComponent<ProjectileData>();
                ref readonly var attackCooldownRuntime = ref entity.GetComponent<AttackCooldownRuntime>().Value;
                ref readonly var attackDamageRuntime = ref entity.GetComponent<AttackDamageRuntime>().Value;
                var shootPoint = entity.GetComponent<ShootPoint>().Value.position;

                entity.SetComponent(new IsAttackOnCooldown
                {
                    EstimateTime = attackCooldownRuntime
                });
                
                // Anim
                if (entity.Has<ViewAnimator>())
                {
                    entity.GetComponent<ViewAnimator>().Value.SetTrigger(AnimAttackTrigger);
                }
                
                // Creating aoe cast projectile
                var aoeCastEntity = World.CreateEntity();
                aoeCastEntity.InstantiateView(projectileParams.EntityView, shootPoint, Quaternion.identity);
                aoeCastEntity.AddComponent<AoeCastTag>();
                aoeCastEntity.AddComponent<ProjectileTag>();
                aoeCastEntity.SetComponent(new AttackRangeRuntime
                {
                    Value = attackRangeRuntime
                });
                
                // Critical chance or flat damage
                var damage = attackDamageRuntime;
                var isCritical = false;
                if (entity.Has<CriticalChanceRuntime>() && entity.Has<CriticalDamageRuntime>())
                {
                    ref readonly var criticalChanceRuntime = ref entity.GetComponent<CriticalChanceRuntime>().Value;
                    if (Random.Range(0f, 1f) < 1f - criticalChanceRuntime)
                    {
                        ref readonly var criticalDamageRuntime = ref entity.GetComponent<CriticalDamageRuntime>().Value;
                        damage += attackDamageRuntime * criticalDamageRuntime;
                        isCritical = true;
                    }
                }
                aoeCastEntity.SetComponent(new PerformingDamage
                {
                    Value = damage,
                    IsCritical = isCritical
                });
                
                // Land
                if (entity.Has<TowerAttackLandsToTheGroundEnemy>())
                {
                    aoeCastEntity.AddComponent<TowerAttackLandsToTheGroundEnemy>();
                }
                
                // Focus
                if (entity.Has<FocusGroundEnemiesTag>())
                {
                    aoeCastEntity.AddComponent<FocusGroundEnemiesTag>();
                }
                if (entity.Has<FocusFlyingEnemiesTag>())
                {
                    aoeCastEntity.AddComponent<FocusFlyingEnemiesTag>();
                }
                
                if (entity.Has<DelayToPerformAttack>())
                {
                    aoeCastEntity.SetComponent(new IsOnDelayToPerformAttack
                    {
                        Value = entity.GetComponent<DelayToPerformAttack>().Value 
                    });
                }
                
                if (entity.Has<ShootVfx>())
                {
                    aoeCastEntity.SetComponent(entity.GetComponent<ShootVfx>());
                }
                if (entity.Has<HitVfx>())
                {
                    aoeCastEntity.SetComponent(entity.GetComponent<HitVfx>());
                }
            }
        }

        public void Dispose()
        {
        }
    }
}