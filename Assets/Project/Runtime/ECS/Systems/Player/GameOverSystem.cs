using Project.Runtime.ECS.Components;
using Project.Runtime.Features.GameplayMenus;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    public class GameOverSystem : ISystem
    {
        [Inject] private GameFinishedPanel gameFinishedPanel;
        
        public World World { get; set; }

        private Filter _filter;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<BaseTowerTag>()
                .With<DestroyedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_filter.IsEmpty()) return;
            Debug.Log("GAME OVER ");
            
            gameFinishedPanel.Show();

            World.UpdateByUnity = false;
        }

        public void Dispose()
        {
        }
    }
}