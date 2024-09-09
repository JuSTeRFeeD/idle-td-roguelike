using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    public class BaseTowerApplyDamageSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<BaseTowerTag>()
                .With<HealthCurrent>()
                .With<DamageAccumulator>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var damageAccum = ref entity.GetComponent<DamageAccumulator>();
                ref var health = ref entity.GetComponent<HealthCurrent>().Value;

                health -= damageAccum.DamagersAmount;
                entity.RemoveComponent<DamageAccumulator>();

                if (health > 0)
                {
                    SpawnHealthbarIfNotSpawned(entity);
                    continue;
                }
                
                entity.SetComponent(new ToDestroyTag());
            }
        }
        
        // same code in ApplyDamageSystem
        private void SpawnHealthbarIfNotSpawned(in Entity entity)
        {
            if (entity.Has<HealthbarEntityRef>()) return;
            
            var hbEntity = World.CreateEntity();
            hbEntity.SetComponent(new Owner()
            {
                Entity = entity
            });
            hbEntity.SetComponent(new HealthbarTag());
            hbEntity.InstantiateView(_worldSetup.WorldHealthBarView, entity.ViewPosition(), Quaternion.identity);
            
            entity.SetComponent(new HealthbarEntityRef
            {
                Value = hbEntity
            });
        }

        public void Dispose()
        {
        }
    }
}