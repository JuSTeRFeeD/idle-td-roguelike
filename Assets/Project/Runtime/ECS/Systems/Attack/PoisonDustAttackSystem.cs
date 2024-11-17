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
    public class PoisonDustAttackSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<PoisonDustTag>()
                .With<SplashDamageRuntime>()
                .With<AttackCooldownRuntime>()
                .Without<IsAttackOnCooldown>()
                .Without<ToDestroyTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var poisonSplashDamage = World.CreateEntity();
                poisonSplashDamage.InstantiateView(_worldSetup.NullView, entity.ViewPosition(), Quaternion.identity);
                poisonSplashDamage.AddComponent<ProjectileTag>();
                poisonSplashDamage.AddComponent<PoisonDustProjectileTag>();
                poisonSplashDamage.SetComponent(new ProjectileHit
                {
                    HitEntity = entity
                });
                poisonSplashDamage.SetComponent(entity.GetComponent<SplashDamageRuntime>());
                poisonSplashDamage.SetComponent(entity.GetComponent<AttackDamageRuntime>());
                poisonSplashDamage.SetComponent(new ToDestroyTag());
                
                entity.SetComponent(new IsAttackOnCooldown
                {
                    EstimateTime = entity.GetComponent<AttackCooldownRuntime>().Value
                });

                Debug.Log($"Dust attack from {entity.ID.ToString()} | ProjectileId: {poisonSplashDamage.ID.ToString()} | cd {entity.GetComponent<AttackCooldownRuntime>().Value}");
            }
        }

        public void Dispose()
        {
            
        }
    }
}