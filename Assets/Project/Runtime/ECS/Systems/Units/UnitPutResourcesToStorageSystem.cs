using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitPutResourcesToStorageSystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitsFilter;
        
        public void OnAwake()
        {
            _unitsFilter = World.Filter
                .With<UnitTag>()
                .With<UnitBackpack>()
                .With<MoveToStorage>()
                .With<MoveToTargetCompleted>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _unitsFilter)
            {
                ref readonly var storageEntity = ref entity.GetComponent<MoveToStorage>().Entity;
                ref var backpack = ref entity.GetComponent<UnitBackpack>();
                
                if (storageEntity.Has<WoodStorage>())
                {
                    PutResources(ref storageEntity.GetComponent<WoodStorage>(), ref backpack.WoodAmount);
                }
                if (storageEntity.Has<StoneStorage>())
                {
                    PutResources(ref storageEntity.GetComponent<StoneStorage>(), ref backpack.StoneAmount);
                }
                
                entity.RemoveComponent<MoveToStorage>();
                if (backpack.WoodAmount > 0 || backpack.StoneAmount > 0)
                {
                    entity.SetComponent(new FindStorageRequest());
                }
                else
                {
                    entity.SetComponent(new FindResourceRequest());
                }
            }
        }

        private static void PutResources<T>(ref T storage, ref int backpackAmount) where T : struct, IStorage
        {
            var possiblePutIntoStorage = storage.Max - storage.Current;
            
            if (possiblePutIntoStorage >= backpackAmount)
            {
                storage.Current += backpackAmount;
                backpackAmount = 0;
            }
            else
            {
                storage.Current += possiblePutIntoStorage;
                backpackAmount -= possiblePutIntoStorage;
            }
        }

        public void Dispose()
        {
        }
    }
}