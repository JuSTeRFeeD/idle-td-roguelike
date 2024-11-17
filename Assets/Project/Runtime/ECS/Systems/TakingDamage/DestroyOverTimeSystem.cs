using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class DestroyOverTimeSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<DestroyOverTime> _destroyOverTimeStash;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<DestroyOverTime>()
                .Build();

            _destroyOverTimeStash = World.GetStash<DestroyOverTime>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var time = ref _destroyOverTimeStash.Get(entity);
                time.EstimateTime -= deltaTime;
                if (time.EstimateTime > 0) continue;

                if (entity.Has<PoisonDustTag>())
                {
                    Debug.Log($"Destroyed {entity.ID.ToString()} PoisonDustTag");
                }
                if (entity.Has<PoisonDustProjectileTag>())
                {
                    Debug.Log($"Destroyed {entity.ID.ToString()} PoisonDustTag");
                }
                
                entity.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}