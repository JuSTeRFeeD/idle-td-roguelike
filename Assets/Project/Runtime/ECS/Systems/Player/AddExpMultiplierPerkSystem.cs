using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class AddExpMultiplierPerkSystem : ISystem
    {
        public World World { get; set; }

        private Filter _addExpFilter;
        private Filter _addExpMultiplierFilter;
        
        public void OnAwake()
        {
            _addExpFilter = World.Filter
                .With<PlayerAddExp>()
                .Build();

            _addExpMultiplierFilter = World.Filter
                .With<ExpGainIncreasePerk>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var perkEntity in _addExpMultiplierFilter)
            {
                var multiplier = perkEntity.GetComponent<ExpGainIncreasePerk>().Multiplier;
                foreach (var entity in _addExpFilter)
                {
                    ref var addExp = ref entity.GetComponent<PlayerAddExp>();
                    addExp.Value *= multiplier;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}