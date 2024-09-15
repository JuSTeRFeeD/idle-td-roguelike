using Project.Runtime.ECS.Components;
using Project.Runtime.Features;
using Project.Runtime.Features.GameplayMenus;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.GameCycle
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class DayNightSystem : ISystem
    {
        [Inject] private HeaderUI _headerUI;
        [Inject] private DayNightCycleEffects _dayNightCycleEffects;
        
        [Inject] private WorldSetup _worldSetup;
        [Inject] private GameFinishedPanel _gameFinishedPanel;
        
        public World World { get; set; }

        private Filter _filter;
        private Stash<DayNight> _dayNightStash;
        private Stash<IsDayTimeTag> _isDayTimeTagStash;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<DayNight>()
                .Without<IsNightTimeTag>()
                .Build();
            
            _dayNightStash = World.GetStash<DayNight>();
            _isDayTimeTagStash = World.GetStash<IsDayTimeTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            // как только убиваем всех врагов -> ставится таймер на 3с и возвращается день
            
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

                    // WIN
                    // dayNight.DayNumber - 1 cuz day number starts from 1
                    if (_worldSetup.NightWavesConfig.WavesCount == dayNight.DayNumber - 1) 
                    {
                        _gameFinishedPanel.Show();
                    }
                }
                
                _headerUI.SetDayNight(dayNight, !isPreviousDay);
            }
        }

        public void Dispose()
        {
        }
    }
}