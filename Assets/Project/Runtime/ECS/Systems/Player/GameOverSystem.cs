using Project.Runtime.ECS.Components;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.TimeManagement;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class GameOverSystem : ISystem
    {
        [Inject] private GameFinishedPanel _gameFinishedPanel;
        
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
            
            _gameFinishedPanel.Show();

            World.UpdateByUnity = false;
            TimeScale.SetTimeScale(1f);
        }

        public void Dispose()
        {
        }
    }
}