using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems
{
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
        }

        public void Dispose()
        {
        }
    }
}