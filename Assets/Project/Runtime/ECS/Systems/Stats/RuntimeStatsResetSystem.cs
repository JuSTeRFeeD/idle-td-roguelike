using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Stats
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class RuntimeStatsResetSystem : ISystem
    {
        public World World { get; set; }

        private Filter _moveSpeedFilter;
        private Stash<MoveSpeed> _moveSpeedStash;
        private Stash<MoveSpeedRuntime> _moveSpeedRuntimeStash;

        private Filter _attackRangeFilter;
        private Stash<AttackRange> _attackRangeStash;
        private Stash<AttackRangeRuntime> _attackRangeRuntimeStash;

        private Filter _attackDamageFilter;
        private Stash<AttackDamage> _attackDamageStash;
        private Stash<AttackDamageRuntime> _attackDamageRuntimeStash;

        private Filter _attackCooldownFilter;
        private Stash<AttackCooldown> _attackCooldownStash;
        private Stash<AttackCooldownRuntime> _attackCooldownRuntimeStash;

        private Filter _criticalChanceDamageFilter;
        private Stash<CriticalChance> _criticalChanceStash;
        private Stash<CriticalChanceRuntime> _criticalChanceRuntimeStash;
        private Stash<CriticalDamage> _criticalDamageStash;
        private Stash<CriticalDamageRuntime> _criticalDamageRuntimeStash;

        private Filter _projectileBouncesFilter;
        private Stash<TowerWithBouncingProjectile> _towerWithBouncingProjectileStash;
        private Stash<TowerWithBouncingProjectileRuntime> _towerWithBouncingProjectileRuntimeStash;

        public void OnAwake()
        {
            _moveSpeedFilter = World.Filter.With<MoveSpeed>().With<MoveSpeedRuntime>().Build();
            _moveSpeedStash = World.GetStash<MoveSpeed>();
            _moveSpeedRuntimeStash = World.GetStash<MoveSpeedRuntime>();

            _attackRangeFilter = World.Filter.With<AttackRange>().With<AttackRangeRuntime>().Build();
            _attackRangeStash = World.GetStash<AttackRange>();
            _attackRangeRuntimeStash = World.GetStash<AttackRangeRuntime>();

            _attackDamageFilter = World.Filter.With<AttackDamage>().With<AttackDamageRuntime>().Build();
            _attackDamageStash = World.GetStash<AttackDamage>();
            _attackDamageRuntimeStash = World.GetStash<AttackDamageRuntime>();

            _attackCooldownFilter = World.Filter.With<AttackCooldown>().With<AttackCooldownRuntime>().Build();
            _attackCooldownStash = World.GetStash<AttackCooldown>();
            _attackCooldownRuntimeStash = World.GetStash<AttackCooldownRuntime>();

            _criticalChanceDamageFilter = World.Filter
                .With<CriticalDamage>().With<CriticalChance>()
                .With<CriticalDamageRuntime>().With<CriticalChanceRuntime>()
                .Build();
            _criticalChanceStash = World.GetStash<CriticalChance>();
            _criticalChanceRuntimeStash = World.GetStash<CriticalChanceRuntime>();;
            _criticalDamageStash = World.GetStash<CriticalDamage>();;
            _criticalDamageRuntimeStash = World.GetStash<CriticalDamageRuntime>();;

            _projectileBouncesFilter = World.Filter
                .With<TowerWithBouncingProjectile>()
                .With<TowerWithBouncingProjectileRuntime>()
                .Build();
            _towerWithBouncingProjectileStash = World.GetStash<TowerWithBouncingProjectile>();
            _towerWithBouncingProjectileRuntimeStash = World.GetStash<TowerWithBouncingProjectileRuntime>();
        }

        public void OnUpdate(float deltaTime)
        {
            // Reset MoveSpeedRuntime
            foreach (var entity in _moveSpeedFilter)
            {
                _moveSpeedRuntimeStash.Get(entity).Value = _moveSpeedStash.Get(entity).Value;
            }

            // Reset AttackRangeRuntime
            foreach (var entity in _attackRangeFilter)
            {
                _attackRangeRuntimeStash.Get(entity).Value = _attackRangeStash.Get(entity).Value;
            }

            // Reset AttackDamageRuntime
            foreach (var entity in _attackDamageFilter)
            {
                _attackDamageRuntimeStash.Get(entity).Value = _attackDamageStash.Get(entity).Value;
            }

            // Reset AttackCooldownRuntime
            foreach (var entity in _attackCooldownFilter)
            {
                _attackCooldownRuntimeStash.Get(entity).Value = _attackCooldownStash.Get(entity).Value;
            }

            // Reset Bounces count runtime
            foreach (var entity in _projectileBouncesFilter)
            {
                _towerWithBouncingProjectileRuntimeStash.Get(entity).Bounces =
                    _towerWithBouncingProjectileStash.Get(entity).Bounces;
            }

            // Reset CriticalChance & CriticalDamage
            foreach (var entity in _criticalChanceDamageFilter)
            {
                _criticalChanceRuntimeStash.Get(entity).Value = _criticalChanceStash.Get(entity).Value;
                _criticalDamageRuntimeStash.Get(entity).Value = _criticalDamageStash.Get(entity).Value;
            }
        }

        public void Dispose()
        {
        }
    }
}