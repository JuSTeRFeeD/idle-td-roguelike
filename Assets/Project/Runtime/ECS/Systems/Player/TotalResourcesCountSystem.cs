using Project.Runtime.ECS.Components;
using Project.Runtime.Features;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class TotalResourcesCountSystem : ISystem
    {
        [Inject] private HeaderUI _headerUI;
        
        public World World { get; set; }

        private Filter _filter;
        private Filter _woodStorageFilter;
        private Filter _stoneStorageFilter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<TotalResourcesData>()
                .Build();

            _woodStorageFilter = World.Filter
                .With<WoodStorage>()
                .Build();
            _stoneStorageFilter = World.Filter
                .With<StoneStorage>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // TODO: производить пересчет при изменении данных
            
            foreach (var entity in _filter)
            {
                var woodAmount = 0;
                var woodCapacity = 0;
                var stoneAmount = 0;
                var stoneCapacity = 0;
                
                foreach (var wood in _woodStorageFilter)
                {
                    ref readonly var storage = ref wood.GetComponent<WoodStorage>();
                    woodAmount += storage.Current;
                    woodCapacity += storage.Max;
                }
                foreach (var stone in _stoneStorageFilter)
                {
                    ref readonly var storage = ref stone.GetComponent<StoneStorage>();
                    stoneAmount += storage.Current;
                    stoneCapacity += storage.Max;
                }

                ref var data = ref entity.GetComponent<TotalResourcesData>();
                data.WoodAmount = woodAmount;
                data.WoodCapacity = woodCapacity;
                data.StoneAmount = stoneAmount;
                data.StoneCapacity = stoneCapacity;
                
                _headerUI.SetResourcesAmount(data);
            }
        }

        public void Dispose()
        {
        }
    }
}