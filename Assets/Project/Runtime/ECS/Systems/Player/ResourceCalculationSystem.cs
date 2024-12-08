using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class ResourceCalculationSystem : ISystem
    {
        [Inject] private ResourceCounter _resourceCounter;
        
        public World World { get; set; }

        private Filter _woodFilter;
        private Filter _stoneFilter;
        private Stash<WoodStorage> _woodStash;
        private Stash<StoneStorage> _stoneStash;
        
        public void OnAwake()
        {
            _resourceCounter.WoodAmount = 0;
            _resourceCounter.StoneAmount = 0;
            
            _woodFilter = World.Filter.With<WoodStorage>().Build();
            _stoneFilter = World.Filter.With<StoneStorage>().Build();
            _woodStash = World.GetStash<WoodStorage>();
            _stoneStash= World.GetStash<StoneStorage>();
        }

        public void OnUpdate(float deltaTime)
        {
            var woodAmount = 0;
            var woodCapacity = 0;
            var stoneAmount = 0;
            var stoneCapacity = 0;
            
            foreach (var entity in _woodFilter)
            {
                woodAmount += _woodStash.Get(entity).Current;
                woodCapacity += _woodStash.Get(entity).Max;
            }
            foreach (var entity in _stoneFilter)
            {
                stoneAmount += _stoneStash.Get(entity).Current;
                stoneCapacity += _stoneStash.Get(entity).Max;
            }

            _resourceCounter.WoodAmount = woodAmount;
            _resourceCounter.WoodFull = woodAmount >= woodCapacity;
            _resourceCounter.StoneAmount = stoneAmount;
            _resourceCounter.StoneFull = stoneAmount >= stoneCapacity;
        }

        public void Dispose()
        {
        }
    }
}