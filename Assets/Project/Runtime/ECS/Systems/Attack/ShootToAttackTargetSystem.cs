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
    public class ShootToAttackTargetSystem : ISystem
    {
        [Inject] private VfxSetup _vfxSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<AttackTarget>()
                .With<ShootPoint>()
                .With<ProjectileData>()
                .With<AttackDamageRuntime>()
                .With<AttackCooldownRuntime>()
                .With<AttackRangeRuntime>()
                .Without<IsAttackOnCooldown>()
                .Without<AoeAttackingTowerTag>()
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
                
                // Creating projectile
                var projectileEntity = World.CreateEntity();
                projectileEntity.InstantiateView(projectileParams.EntityView, shootPoint, Quaternion.identity);
                projectileEntity.SetComponent(new ProjectileTag());
                projectileEntity.SetComponent(new TrajectoryProjectile
                {
                    MaxAdditionalHeight = 1.5f
                });
                projectileEntity.SetComponent(new ProjectileMoveData
                {
                    StartMovePosition = shootPoint,
                    TravelTime = 0
                });
                projectileEntity.SetComponent(new MoveSpeedRuntime
                {
                    Value = projectileParams.ProjectileSpeed
                });
                projectileEntity.SetComponent(new AttackDamageRuntime
                {
                    Value = attackDamageRuntime
                });
                projectileEntity.SetComponent(new AttackTarget
                {
                    Value = attackTarget
                });

                if (entity.Has<SplashDamageRuntime>())
                {
                    projectileEntity.SetComponent(entity.GetComponent<SplashDamageRuntime>());
                }

                if (entity.Has<TowerWithBouncingProjectileRuntime>())
                {
                    projectileEntity.SetComponent(new BouncingProjectile
                    {
                        BouncesLeft = entity.GetComponent<TowerWithBouncingProjectileRuntime>().Bounces
                    });
                }
                if (entity.Has<HitVfx>())
                {
                    projectileEntity.SetComponent(entity.GetComponent<HitVfx>());
                }

                // Change target to prevent shooting to the same target
                ref var ghostTargetHealth = ref attackTarget.GetComponent<HealthCurrent>().GhostValue;
                ghostTargetHealth -= attackDamageRuntime;
                if (ghostTargetHealth <= 0)
                {
                    // еще можно вынести в перк в отдельную систему прокачки в лобби
                    // TODO: если тавера будут слишком имбовыми - убрать механнику ghostHealth и WillDeadAtNextTick
                    attackTarget.SetComponent(new WillDeadAtNextTickTag());
                    entity.RemoveComponent<AttackTarget>();
                }

                if (entity.Has<ShootVfx>())
                {
                    VfxPool.Spawn(entity.GetComponent<ShootVfx>().Value, shootPoint);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}