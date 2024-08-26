using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitFindStorageSystem : ISystem
    {
        public World World { get; set; }

        private Filter _lumberjackFilter;
        private Filter _woodStorageFilter;
        
        private Filter _minerFilter;
        private Filter _stoneStorageFilter;
        
        public void OnAwake()
        {
            _lumberjackFilter = World.Filter
                .With<FindStorageRequest>()
                .With<UnitTag>()
                .With<LumberjackTag>()
                .With<UnitBackpack>()
                .With<ViewEntity>()
                .Build();
            _woodStorageFilter = World.Filter
                .With<WoodStorage>()
                .With<ViewEntity>()
                .Build();
            
            _minerFilter = World.Filter
                .With<FindStorageRequest>()
                .With<UnitTag>()
                .With<MinerTag>()
                .With<UnitBackpack>()
                .With<ViewEntity>()
                .Build();
            _stoneStorageFilter = World.Filter
                .With<StoneStorage>()
                .With<ViewEntity>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            FindStorage<WoodStorage>(_lumberjackFilter, _woodStorageFilter);
            FindStorage<StoneStorage>(_minerFilter, _stoneStorageFilter);
        }

        private static void FindStorage<T>(in Filter unitFilter, in Filter storageFilter) where T : struct, IStorage
        {
            foreach (var unitEntity in unitFilter)
            {
                foreach (var storageEntity in storageFilter)
                {
                    ref var storage = ref storageEntity.GetComponent<T>();
                    if (storage.Current >= storage.Max) continue;

                    unitEntity.SetComponent(new MoveToStorage
                    {
                        Entity = storageEntity
                    });
                    unitEntity.RemoveComponent<FindStorageRequest>();
                }
            }
        }

        public void Dispose()
        {
        }
    }
}