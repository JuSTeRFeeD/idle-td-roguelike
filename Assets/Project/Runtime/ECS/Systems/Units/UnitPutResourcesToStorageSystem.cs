using System;
using Project.Runtime.ECS.Components;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Units
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
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
                    PutResources(storageEntity, ref storageEntity.GetComponent<WoodStorage>(), ref backpack.WoodAmount, ResourceType.Wood);
                }
                if (storageEntity.Has<StoneStorage>())
                {
                    PutResources(storageEntity, ref storageEntity.GetComponent<StoneStorage>(), ref backpack.StoneAmount, ResourceType.Stone);
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

        private static void PutResources<T>(in Entity storageEntity, ref T storage, ref int backpackAmount, ResourceType resourceType) where T : struct, IStorage
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

            if (storage.Current >= storage.Max)
            {
                switch (resourceType)
                {
                    case ResourceType.Wood:
                        storageEntity.SetComponent(new WoodStorageFullTag());
                        break;
                    case ResourceType.Stone:
                        storageEntity.SetComponent(new StoneStorageFullTag());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}