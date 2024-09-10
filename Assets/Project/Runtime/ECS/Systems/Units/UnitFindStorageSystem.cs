using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitFindStorageSystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitsFilter;
        private Filter _woodStorageFilter;
        private Filter _stoneStorageFilter;

        public void OnAwake()
        {
            _unitsFilter = World.Filter
                .With<FindStorageRequest>()
                .With<UnitTag>()
                .With<UnitBackpack>()
                .With<ViewEntity>()
                .Build();

            _woodStorageFilter = World.Filter
                .With<WoodStorage>()
                .With<ViewEntity>()
                .Build();
            _stoneStorageFilter = World.Filter
                .With<StoneStorage>()
                .With<ViewEntity>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _unitsFilter)
            {
                ref readonly var backpack = ref entity.GetComponent<UnitBackpack>();
                if (backpack.WoodAmount > 0) FindStorage<WoodStorage>(entity, _woodStorageFilter);
                else if (backpack.StoneAmount > 0) FindStorage<StoneStorage>(entity, _stoneStorageFilter);
            }
        }

        private static void FindStorage<T>(in Entity unitEntity, in Filter storageFilter) where T : struct, IStorage
        {
            foreach (var storageEntity in storageFilter)
            {
                ref var storage = ref storageEntity.GetComponent<T>();
                if (storage.Current >= storage.Max) continue;

                unitEntity.SetComponent(new AStarCalculatePathRequest
                {
                    Entity = storageEntity,
                    TargetPosition = storageEntity.ViewPosition()
                });
                unitEntity.SetComponent(new MoveToStorage
                {
                    Entity = storageEntity
                });
                unitEntity.RemoveComponent<FindStorageRequest>();
            }
        }

        public void Dispose()
        {
        }
    }
}