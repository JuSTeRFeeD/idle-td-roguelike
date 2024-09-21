using System;
using System.Linq;
using NTC.Pool;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Views;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Projectile
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class BounceProjectileSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Filter _enemiesFilter;

        private const float BounceDistance = 2.25f;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<ProjectileTag>()
                .With<ProjectileHit>()
                .With<BouncingProjectile>()
                .Build();
            
            _enemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .Without<ToDestroyTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var bouncingProjectile = ref entity.GetComponent<BouncingProjectile>();
                if (bouncingProjectile.BouncesLeft <= 0)
                {
                    continue;
                }
                
                ref readonly var hitEnemy = ref entity.GetComponent<ProjectileHit>().HitEntity;

                if (bouncingProjectile.BouncedEntities == null)
                {
                    bouncingProjectile.BouncedEntities = new[] { hitEnemy };
                }
                else
                {
                    Array.Resize(ref bouncingProjectile.BouncedEntities, bouncingProjectile.BouncedEntities.Length + 1);
                    bouncingProjectile.BouncedEntities[^1] = hitEnemy;
                }

                var bounced = bouncingProjectile.BouncedEntities.ToHashSet();
                var projectilePos = entity.ViewPosition();
                    
                foreach (var enemyEntity in _enemiesFilter)
                {
                    if (bounced.Contains(enemyEntity)) continue;
                    var enemyPos = enemyEntity.ViewPosition();

                    if (Vector3.SqrMagnitude(enemyPos - projectilePos) > BounceDistance * BounceDistance)
                    {
                        continue;
                    }

                    bouncingProjectile.BouncesLeft--;
                    
                    // Spawn bounced projectile
                    var newProjectile = World.CreateEntity();
                    newProjectile.InstantiateView(
                    NightPool.GetPoolByClone(entity.GetComponent<ViewEntity>().Value).AttachedPrefab.GetComponent<EntityView>(), 
                    projectilePos,
                    Quaternion.identity);
                    newProjectile.AddComponent<ProjectileTag>();
                    newProjectile.SetComponent(entity.GetComponent<BouncingProjectile>());
                    newProjectile.SetComponent(entity.GetComponent<TrajectoryProjectile>());
                    newProjectile.SetComponent(entity.GetComponent<MoveSpeedRuntime>());
                    newProjectile.SetComponent(entity.GetComponent<AttackDamageRuntime>());
                    newProjectile.SetComponent(new AttackTarget
                    {
                    Value = enemyEntity
                    });
                    var projectileMoveData = entity.GetComponent<ProjectileMoveData>();
                    projectileMoveData.StartMovePosition = projectilePos;
                    projectileMoveData.TravelTime = 0f;
                    newProjectile.SetComponent(projectileMoveData);
                    
                    break;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}