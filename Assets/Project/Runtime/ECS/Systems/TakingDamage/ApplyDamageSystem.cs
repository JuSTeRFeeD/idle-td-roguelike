using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    public class ApplyDamageSystem : ISystem
    {
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
                
                Debug.Log($"Taked damage {damageAccumulator}. CurHP {healthCurrent}");
                
                if (healthCurrent > 0) continue;
                
                entity.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}