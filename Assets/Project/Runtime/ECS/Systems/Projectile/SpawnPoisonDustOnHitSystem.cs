using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Projectile
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class SpawnPoisonDustOnHitSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<ProjectileTag>()
                .With<ProjectileHit>()
                .With<SpawnPoisonDustOnHit>()
                .With<PoisonDustDataRuntime>()
                .Without<ToDestroyTag>()
                .Without<DestroyOverTime>()
                .Build();
        }

        // Когда projectile попадает во врага и имеет компонент SpawnPoisonDustOnHit - Спавнит облако
        
        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var poisonVfx = ref entity.GetComponent<SpawnPoisonDustOnHit>().PoisonVFX;
                ref readonly var poison = ref entity.GetComponent<PoisonDustDataRuntime>();
                
                VfxPool.Spawn(poisonVfx, entity.ViewPosition());
                
                var poisonDust = World.CreateEntity();
                poisonDust.InstantiateView(_worldSetup.NullView, entity.ViewPosition(), Quaternion.identity);
                poisonDust.AddComponent<PoisonDustTag>();
                poisonDust.SetComponent(new DestroyOverTime
                {
                    EstimateTime = poison.Lifetime
                });
                poisonDust.SetComponent(new AttackCooldownRuntime
                {
                    Value = poison.TimeBetweenAttack
                });
                poisonDust.SetComponent(new PerformingDamage
                {
                    Value = poison.Damage
                });
                poisonDust.SetComponent(new SplashDamageRuntime
                {
                    Radius = poison.Radius,
                    PercentFromDamage = 1f
                });
                Debug.Log($"Created poison {entity.ID.ToString()}");
            }
        }

        public void Dispose()
        {
        }
    }
}