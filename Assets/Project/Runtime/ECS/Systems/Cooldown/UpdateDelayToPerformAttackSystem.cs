using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Cooldown
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class UpdateDelayToPerformAttackSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;

        private Stash<IsOnDelayToPerformAttack> _isOnDelayToPerformAttackStash;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<IsOnDelayToPerformAttack>()
                .Build();

            _isOnDelayToPerformAttackStash = World.GetStash<IsOnDelayToPerformAttack>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var delay = ref _isOnDelayToPerformAttackStash.Get(entity);
                
                delay.Value -= deltaTime;
                if (delay.Value > 0) continue;
                
                _isOnDelayToPerformAttackStash.Remove(entity);
            }
        }

        public void Dispose()
        {
        }
    }
}