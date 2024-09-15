using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Units
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class UnitSetFindResWhenNeedSystem : ISystem
    {
        // 
        // Юниты будут искать ресы если есть хоть один storage который не заполнен
        // И они больше ни чем не заняты
        //

        public World World { get; set; }

        private Filter _chillingUnitsFilter;
        private Filter _woodStorageFilter;
        private Filter _stoneStorageFilter;
        
        public void OnAwake()
        {
            _chillingUnitsFilter = World.Filter
                .With<UnitTag>()
                .Without<AStarPath>()
                .Without<FindResourceRequest>()
                .Without<MoveToResource>()
                .Without<FindStorageRequest>()
                .Without<MoveToStorage>()
                .Without<Gathering>()
                .Without<UnitRepairingTower>()
                .Without<UnitMoveToRepairTower>()
                .Build();

            _woodStorageFilter = World.Filter.With<WoodStorage>().Without<WoodStorageFullTag>().Build();
            _stoneStorageFilter = World.Filter.With<StoneStorage>().Without<StoneStorageFullTag>().Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_woodStorageFilter.IsEmpty() && _stoneStorageFilter.IsEmpty()) return;
            foreach (var entity in _chillingUnitsFilter)
            {
                entity.SetComponent(new FindResourceRequest());
            }
        }

        public void Dispose()
        {
        }
    }
}