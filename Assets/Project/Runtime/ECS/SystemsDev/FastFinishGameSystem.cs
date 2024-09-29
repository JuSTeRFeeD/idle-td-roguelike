#if UNITY_EDITOR
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Helpers;
using UnityEngine;

namespace Project.Runtime.ECS.SystemsDev
{
    public class FastFinishGameSystem : ISystem
    {
        public World World { get; set; }

        private Filter _baseTowerFilter;
        private Filter _dayNightFilter;
        
        public void OnAwake()
        {
            _baseTowerFilter = World.Filter
                .With<BaseTowerTag>()
                .Build();
            _dayNightFilter = World.Filter
                .With<DayNight>()
                .Without<GameFinishedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // Lose
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _baseTowerFilter.First().AddOrGet<DamageAccumulator>().DamagersAmount = 10;
            }
            
            // Win
            if (Input.GetKeyDown(KeyCode.W))
            {
                foreach (var entity in _dayNightFilter)
                {
                    entity.AddComponent<GameFinishedTag>();
                    entity.AddComponent<GameWinTag>();
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
#endif