using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitPutResourcesToStorageSystem : ISystem
    {
        public World World { get; set; }

        private Filter _lumberjackFilter;
        private Filter _minerFilter;
        
        public void OnAwake()
        {
            _lumberjackFilter = World.Filter
                .With<UnitTag>()
                .With<LumberjackTag>()
                .With<UnitBackpack>()
                .With<MoveToStorage>()
                .With<MoveToTargetCompleted>()
                .Build();
            
            _minerFilter = World.Filter
                .With<UnitTag>()
                .With<MinerTag>()
                .With<UnitBackpack>()
                .With<MoveToStorage>()
                .With<MoveToTargetCompleted>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            PutResources<WoodStorage>(_lumberjackFilter);
            PutResources<StoneStorage>(_minerFilter);
        }

        private static void PutResources<T>(in Filter unitFilter) where T : struct, IStorage
        {
            foreach (var entity in unitFilter)
            {
                ref var backpack = ref entity.GetComponent<UnitBackpack>(); 
                ref var storage = ref entity.GetComponent<MoveToStorage>().Entity.GetComponent<T>();

                var possiblePutIntoStorage = storage.Max - storage.Current;
                
                if (possiblePutIntoStorage >= backpack.Amount)
                {
                    storage.Current += backpack.Amount;
                    backpack.Amount = 0;
                    entity.RemoveComponent<MoveToStorage>();
                    entity.SetComponent(new FindResourceRequest());
                }
                else
                {
                    storage.Current += possiblePutIntoStorage;
                    backpack.Amount -= possiblePutIntoStorage;
                    entity.RemoveComponent<MoveToStorage>();
                    entity.SetComponent(new FindStorageRequest());
                }
            }
        }

        public void Dispose()
        {
        }
    }
}