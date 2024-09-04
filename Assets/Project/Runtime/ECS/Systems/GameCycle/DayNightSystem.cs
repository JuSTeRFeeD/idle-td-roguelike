using Project.Runtime.ECS.Components;
using Project.Runtime.Features;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.GameCycle
{
    public class DayNightSystem : ISystem
    {
        [Inject] private HeaderUI _headerUI;
        [Inject] private DayNightCycleEffects _dayNightCycleEffects;
        
        public World World { get; set; }

        private Filter _filter;
        private Stash<DayNight> _dayNightStash;
        private Stash<IsDayTimeTag> _isDayTimeTagStash;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<DayNight>()
                .Build();
            
            _dayNightStash = World.GetStash<DayNight>();
            _isDayTimeTagStash = World.GetStash<IsDayTimeTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var dayNight = ref _dayNightStash.Get(entity);
                
                dayNight.EstimateTime -= deltaTime;

                if (dayNight.EstimateTime > 0f)
                {
                    _headerUI.SetDayNight(dayNight, _isDayTimeTagStash.Has(entity));

                    if (dayNight.EstimateTime <= _dayNightCycleEffects.transitionDuration)
                    {
                        _dayNightCycleEffects.SetTime(!_isDayTimeTagStash.Has(entity));
                    }
                    
                    continue;
                }

                var isPreviousDay = _isDayTimeTagStash.Has(entity);
                if (isPreviousDay)
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
                
                _headerUI.SetDayNight(dayNight, !isPreviousDay);
            }
        }

        public void Dispose()
        {
        }
    }
}