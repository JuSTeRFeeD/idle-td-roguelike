using Project.Runtime.ECS.Components;
using Project.Runtime.Features;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.GameCycle
{
    public class DayNightSystem : ISystem
    {
        [Inject] private HeaderUI _headerUI;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<DayNight>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var dayNight = ref entity.GetComponent<DayNight>();
                
                dayNight.EstimateTime -= deltaTime;

                if (dayNight.EstimateTime > 0f)
                {
                    _headerUI.SetDayNight(dayNight, entity.Has<IsDayTimeTag>());
                    continue;
                }

                if (entity.Has<IsDayTimeTag>())
                {
                    dayNight.EstimateTime = dayNight.NightTime;
                    entity.RemoveComponent<IsDayTimeTag>();
                    entity.AddComponent<IsNightTimeTag>();
                }
                else
                {
                    dayNight.DayNumber++;
                    dayNight.EstimateTime = dayNight.DayTime;
                    entity.RemoveComponent<IsNightTimeTag>();
                    entity.AddComponent<IsDayTimeTag>();
                }
                
                _headerUI.SetDayNight(dayNight, entity.Has<IsDayTimeTag>());
            }
        }

        public void Dispose()
        {
        }
    }
}