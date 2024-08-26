using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

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
                .With<MoveToTargetComplete>()
                .Build();
            _minerFilter = World.Filter
                .With<UnitTag>()
                .With<MinerTag>()
                .With<UnitBackpack>()
                .With<MoveToStorage>()
                .With<MoveToTargetComplete>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            PutResources<WoodStorage>(_lumberjackFilter);
            PutResources<StoneStorage>(_minerFilter);
        }

        private static void PutResources<T>(Filter unitFilter) where T : struct, IStorage
        {
            var buff = new Dictionary<Entity, int>();
            
            foreach (var entity in unitFilter)
            {
                ref var backpack = ref entity.GetComponent<UnitBackpack>(); 
                var storageEntity = entity.GetComponent<MoveToStorage>().Entity;
                ref var storage = ref entity.GetComponent<MoveToStorage>().Entity.GetComponent<T>();

                var possiblePutIntoStorage = storage.Max - storage.Current;
                
                // TODO: чекнуть почему при большом кол-ве юнитов в storage current может превысить max
                // if (buff.TryGetValue(storageEntity, out var curInBuff)) // вроде как это реально нужно нам
                // {
                    // possiblePutIntoStorage = storage.Max - curInBuff;
                // }
                
                if (possiblePutIntoStorage >= backpack.Amount)
                {
                    storage.Current += backpack.Amount;
                    backpack.Amount = 0;
                    entity.RemoveComponent<MoveToStorage>();
                    entity.SetComponent(new FindResourceRequest());
                }
                else
                {
                    var possible = backpack.Amount - possiblePutIntoStorage;
                    storage.Current += possible;
                    backpack.Amount -= possible;
                    entity.RemoveComponent<MoveToStorage>();
                    entity.SetComponent(new FindStorageRequest());
                }

                if (buff.ContainsKey(storageEntity)) buff[storageEntity] = storage.Current;
                else buff.Add(storageEntity, storage.Current);
            }
        }

        public void Dispose()
        {
        }
    }
}