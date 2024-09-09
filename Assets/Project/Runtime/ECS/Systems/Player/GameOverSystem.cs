using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Player
{
    public class GameOverSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<BaseTowerTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_filter.IsEmpty()) return;
            Debug.Log("GAME OVER");
        }

        public void Dispose()
        {
        }
    }
}