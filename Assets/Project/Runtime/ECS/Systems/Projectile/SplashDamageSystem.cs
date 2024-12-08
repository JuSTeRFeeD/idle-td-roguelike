using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Systems.FindTarget;
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
        
        private Stash<ViewEntity> _viewEntityStash;
        private Stash<DamageAccumulator> _damageAccumulatorStash;

        private Entity[] _hits = new Entity[128];
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<ProjectileTag>()
                .With<ProjectileHit>()
                .With<SplashDamageRuntime>()
                .With<PerformingDamage>()
                .Build();
            
            _enemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .Without<ToDestroyTag>()
                .Without<DestroyOverTime>()
                .Build();

            _viewEntityStash = World.GetStash<ViewEntity>();
            _damageAccumulatorStash = World.GetStash<DamageAccumulator>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var hitEntity = ref entity.GetComponent<ProjectileHit>().HitEntity;
                if (hitEntity.IsNullOrDisposed()) continue;
                
                ref readonly var performingDamage = ref entity.GetComponent<PerformingDamage>();
                ref readonly var splashDamage = ref entity.GetComponent<SplashDamageRuntime>();

                var count = FindTargetExtension.GetInRangeFilterNoAlloc(hitEntity, _enemiesFilter, splashDamage.Radius, _viewEntityStash, ref _hits);
                
                Debug.Log($"Splash {count} attack from{entity.ID.ToString()} | dmg {performingDamage.Value} radius {splashDamage.Radius} percentFromDamage {splashDamage.PercentFromDamage}");
                
                for (var i = 0; i < count; i++)
                {
                    var hitTo = _hits[i];
                    Debug.Log($"Splash damaged to {hitTo.ID}");
                    ref var damageAccumulator = ref _damageAccumulatorStash.AddOrGet(hitTo);
                    damageAccumulator.Value += performingDamage.Value * splashDamage.PercentFromDamage;
                    damageAccumulator.IsCritical = damageAccumulator.IsCritical || performingDamage.IsCritical;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}