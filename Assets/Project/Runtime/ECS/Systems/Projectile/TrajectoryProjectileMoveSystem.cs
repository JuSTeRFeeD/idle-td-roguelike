using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Projectile
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class TrajectoryProjectileMoveSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<ProjectileTag>()
                .With<TrajectoryProjectile>()
                .With<ProjectileMoveData>()
                .With<MoveSpeedRuntime>()
                .With<AttackDamageRuntime>()
                .With<ViewEntity>()
                .With<AttackTarget>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var projectileEntity in _filter)
            {
                ref readonly var attackTargetEntity = ref projectileEntity.GetComponent<AttackTarget>().Value;
                if (attackTargetEntity.IsNullOrDisposed())
                {
                    projectileEntity.Dispose();
                    continue;
                }

                ref readonly var trajectoryProjectile = ref projectileEntity.GetComponent<TrajectoryProjectile>();
                ref readonly var moveSpeedRuntime = ref projectileEntity.GetComponent<MoveSpeedRuntime>();
                ref var projectileMoveData = ref projectileEntity.GetComponent<ProjectileMoveData>();
                var transform = projectileEntity.ViewTransform();

                projectileMoveData.TravelTime += deltaTime;

                var targetPosition = attackTargetEntity.ViewPosition() + Vector3.up;
                var startMovePosition = projectileMoveData.StartMovePosition;

                var distance = Vector3.Distance(startMovePosition, targetPosition);
                var totalTime = distance / moveSpeedRuntime.Value;

                var progress = Mathf.Clamp01(projectileMoveData.TravelTime / totalTime);

                var currentPosition = Vector3.Lerp(startMovePosition, targetPosition, progress);

                var height = trajectoryProjectile.MaxAdditionalHeight * 4 * progress * (1 - progress);
                currentPosition.y += height;

                transform.position = currentPosition;

                // Проверяем, достигли ли цели
                if (progress < 1f)
                {
                    continue;
                }
                
                projectileEntity.SetComponent(new ProjectileHit
                {
                    HitEntity = attackTargetEntity
                });
            }
        }

        public void Dispose()
        {
        }
    }
}