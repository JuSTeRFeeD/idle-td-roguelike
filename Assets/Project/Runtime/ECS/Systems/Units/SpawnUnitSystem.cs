using System;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Project.Runtime.ECS.Systems.Units
{
    public class SpawnUnitSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<SpawnUnitRequest>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var request = entity.GetComponent<SpawnUnitRequest>();
                
                var rndOffset = Random.insideUnitSphere * .5f;
                rndOffset.y = 0;

                var unitEntity = World.CreateEntity();
                unitEntity.SetComponent(new UnitTag());
                unitEntity.SetComponent(new Owner
                {
                    Entity = request.ForTowerOwner
                });
                
                unitEntity.AddComponent<MoveSpeedRuntime>();
                unitEntity.SetComponent(new MoveSpeed
                {
                    Value = 5f,
                });
                
                unitEntity.SetComponent(new UnitBackpack
                {
                    Capacity = 1,
                    Amount = 0
                });
                
                switch (request.UnitType)
                {
                    case UnitType.Lumberjack:
                        unitEntity.SetComponent(new LumberjackTag());
                        unitEntity.SetComponent(new FindResourceRequest());
                        unitEntity.SetComponent(new GatheringTime
                        {
                            Time = 1f
                        });
                        unitEntity.InstantiateView(_worldSetup.UnitLumberjack, request.AtPosition + rndOffset, Quaternion.identity);
                        break;
                    case UnitType.Miner:
                        unitEntity.SetComponent(new MinerTag());
                        unitEntity.SetComponent(new FindResourceRequest());
                        unitEntity.SetComponent(new GatheringTime
                        {
                            Time = 1f
                        });
                        unitEntity.InstantiateView(_worldSetup.UnitMiner, request.AtPosition + rndOffset, Quaternion.identity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                entity.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}