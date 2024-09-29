using Project.Runtime.ECS.Components;
using Project.Runtime.Features.GameplayMenus;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.GameCycle
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class GameOverWhenBaseDestroyedSystem : ISystem
    {
        [Inject] private GameFinishedPanel _gameFinishedPanel;
        
        public World World { get; set; }

        private Filter _filter;
        private Filter _dayNightFilter;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<BaseTowerTag>()
                .With<DestroyedTag>()
                .Build();
            _dayNightFilter = World.Filter
                .With<DayNight>()
                .Without<GameFinishedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_filter.IsEmpty()) return;

            foreach (var entity in _dayNightFilter)
            {
                entity.AddComponent<GameFinishedTag>();
                entity.AddComponent<GameLoseTag>();
            }
        }

        public void Dispose()
        {
        }
    }
}