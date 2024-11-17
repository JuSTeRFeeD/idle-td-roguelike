using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Helpers;

namespace Project.Runtime.ECS.Systems.Projectile
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class ProjectileHitDealDamageSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<ProjectileTag>()
                .With<ProjectileHit>()
                .With<AttackDamageRuntime>()
                .Without<PoisonDustProjectileTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var hitEntity = ref entity.GetComponent<ProjectileHit>().HitEntity;
                ref readonly var damage = ref entity.GetComponent<AttackDamageRuntime>().Value;
                hitEntity.AddOrGet<DamageAccumulator>().Value += damage;

                entity.RemoveComponent<ProjectileTag>();
                entity.SetComponent(new DestroyOverTime
                {
                    EstimateTime = 0.2f // to complete animations on this view (ex: trails)
                });

                if (entity.Has<HitVfx>())
                {
                    VfxPool.Spawn(entity.GetComponent<HitVfx>().Value, hitEntity.ViewRealModelPosition());
                }
            }
        }

        public void Dispose()
        {
        }
    }
}