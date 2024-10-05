using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class ApplyDamageStatisticSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { set; get; }

        private Filter _statisticsFilter;
        private Stash<TotalDealtDamageStatistic> _totalDealtDamageStash;
        private Stash<TotalKilledEnemiesStatistic> _totalKilledEnemiesStash;
        private Stash<TotalKilledFlyEnemiesStatistic> _totalKilledFlyEnemiesStash;
        private Stash<TotalKilledGroundEnemiesStatistic> _totalKilledGroundEnemiesStash;
    
        private Filter _filter;
        private Stash<DamageAccumulator> _damageAccumulatorStash;
        private Stash<HealthCurrent> _currentHealthStash;
        private Stash<FlyingEnemyTag> _flyEnemyStash;
        private Stash<GroundEnemyTag> _groundEnemyStash;

        public void OnAwake()
        {
            _statisticsFilter = World.Filter
                .With<StatisticTag>()
                .Build();
            _totalDealtDamageStash = World.GetStash<TotalDealtDamageStatistic>();
            _totalKilledEnemiesStash = World.GetStash<TotalKilledEnemiesStatistic>();
            _totalKilledFlyEnemiesStash = World.GetStash<TotalKilledFlyEnemiesStatistic>();
            _totalKilledGroundEnemiesStash = World.GetStash<TotalKilledGroundEnemiesStatistic>();
            
            _filter = World.Filter
                .With<EnemyTag>()
                .With<HealthCurrent>()
                .With<DamageAccumulator>()
                .Build();
            _currentHealthStash = World.GetStash<HealthCurrent>();
            _damageAccumulatorStash = World.GetStash<DamageAccumulator>();
            _flyEnemyStash = World.GetStash<FlyingEnemyTag>();
            _groundEnemyStash = World.GetStash<GroundEnemyTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var damageAccumulator = ref _damageAccumulatorStash.Get(entity).Value;

                var statsEntity = _statisticsFilter.First();
                _totalDealtDamageStash.Get(statsEntity).Value += (int)damageAccumulator;

                if (_currentHealthStash.Get(entity).Value - damageAccumulator > 0)
                {
                    continue;
                }

                _totalKilledEnemiesStash.Get(statsEntity).Value++;
                if (_groundEnemyStash.Has(entity)) _totalKilledGroundEnemiesStash.Get(statsEntity).Value++;
                if (_flyEnemyStash.Has(entity)) _totalKilledFlyEnemiesStash.Get(statsEntity).Value++;
            }
        }

        public void Dispose()
        {
        }
    }
}