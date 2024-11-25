using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.ECS.Systems.Tutorial;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Project.Runtime.ECS.Systems.Units
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class SpawnUnitSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        private Filter _tutorialPreventSpawnFilter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<SpawnUnitRequest>()
                .Build();
            
            _tutorialPreventSpawnFilter = World.Filter.With<TutorialPreventSpawnUnits>().Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_tutorialPreventSpawnFilter.IsNotEmpty()) return;
            
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
                    Value = 3f,
                });
                
                unitEntity.SetComponent(new UnitBackpack
                {
                    Capacity = 3,
                    WoodAmount = 0,
                    StoneAmount = 0
                });
                
                unitEntity.SetComponent(new GatheringTime
                {
                    Time = 1f
                });
                unitEntity.InstantiateView(_worldSetup.WorkerUnitView, request.AtPosition + rndOffset, Quaternion.identity);
                
                entity.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}