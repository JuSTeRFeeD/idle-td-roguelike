using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class BombTakesDamageSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<BombTowerTag>()
                .With<DamageAccumulator>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var bombDamageEntity = World.CreateEntity();
                bombDamageEntity.InstantiateView(_worldSetup.NullView, entity.ViewPosition(), Quaternion.identity);
                bombDamageEntity.SetComponent(entity.GetComponent<AttackDamageRuntime>());
                bombDamageEntity.SetComponent(new ProjectileTag());
                bombDamageEntity.SetComponent(new ProjectileHit
                {
                    HitEntity = bombDamageEntity
                });
                bombDamageEntity.SetComponent(new SplashDamageRuntime
                {
                    Radius = entity.GetComponent<AttackRangeRuntime>().Value,
                    PercentFromDamage = 1f
                });
            }
        }

        public void Dispose()
        {
        }
    }
}