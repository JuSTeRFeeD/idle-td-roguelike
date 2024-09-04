using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Shooting
{
    public class ShootToAttackTargetSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<AttackTarget>()
                .With<ShootPoint>()
                .With<ProjectileParams>()
                .With<AttackDamageRuntime>()
                .With<AttackCooldownRuntime>()
                .With<AttackRangeRuntime>()
                .Without<IsAttackOnCooldown>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var attackRangeRuntime = ref entity.GetComponent<AttackRangeRuntime>().Value;
                ref readonly var attackTarget = ref entity.GetComponent<AttackTarget>().Value;
                
                // Now this entity out of range
                if (Vector3.SqrMagnitude(attackTarget.ViewPosition() - entity.ViewPosition()) >
                    attackRangeRuntime * attackRangeRuntime)
                {
                    entity.RemoveComponent<AttackTarget>();
                    continue;
                }
                
                ref readonly var projectileParams = ref entity.GetComponent<ProjectileParams>();
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
            }
        }

        public void Dispose()
        {
        }
    }
}