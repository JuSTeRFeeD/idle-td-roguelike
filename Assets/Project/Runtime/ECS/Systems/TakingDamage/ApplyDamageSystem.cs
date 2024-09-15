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
    public class ApplyDamageSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { set; get; }
    
        private Filter _filter;
        private Stash<HealthCurrent> _healthCurrentStash;
        private Stash<DamageAccumulator> _damageAccumulatorStash;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<HealthCurrent>()
                .With<DamageAccumulator>()
                .Build();

            _healthCurrentStash = World.GetStash<HealthCurrent>();
            _damageAccumulatorStash = World.GetStash<DamageAccumulator>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var healthCurrent = ref _healthCurrentStash.Get(entity).Value;
                ref var damageAccumulator = ref _damageAccumulatorStash.Get(entity).Value;
                
                healthCurrent -= damageAccumulator;
                
                // TODO: чисто ради эксперимента посмотреть что будет быстрее:
                // 1. удалять компонент (структурное изменение)
                // 2. damageAccumulator = 0 делать (обнуление урона)
                // *во втором случ все существа будут попадать в эту систему после первого получения урона
                entity.RemoveComponent<DamageAccumulator>();
                
                if (healthCurrent > 0)
                {
                    SpawnHealthbarIfNotSpawned(entity);
                    continue;
                }
                
                entity.SetComponent(new ToDestroyTag()); 
                
            }
        }

        // same code in BaseTowerApplyDamageSystem
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