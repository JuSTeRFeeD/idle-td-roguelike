using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Player
{
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
                    Debug.Log($"работает перк увеличения опыта, лол multiplier: {multiplier}");
                }
            }
        }

        public void Dispose()
        {
        }
    }
}