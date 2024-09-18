using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Helpers;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Projectile
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class SplashDamageSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Filter _enemiesFilter;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<ProjectileTag>()
                .With<ProjectileHit>()
                .With<SplashDamageRuntime>()
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
                ref readonly var hitEntity = ref entity.GetComponent<ProjectileHit>().HitEntity;
                ref readonly var damage = ref entity.GetComponent<AttackDamageRuntime>().Value;
                ref readonly var splashDamage = ref entity.GetComponent<SplashDamageRuntime>();

                var hitPoint = hitEntity.ViewPosition();
                foreach (var enemyEntity in _enemiesFilter)
                {
                    var enemyPos = enemyEntity.ViewPosition();
                    if (Vector3.SqrMagnitude(enemyPos - hitPoint) > splashDamage.Radius * splashDamage.Radius)
                    {
                        continue;
                    }

                    enemyEntity.AddOrGet<DamageAccumulator>().Value += damage * splashDamage.PercentFromDamage;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}