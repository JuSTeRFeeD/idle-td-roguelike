using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitGatheringResourceSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<Gathering>()
                .With<GatheringTime>()
                .With<UnitBackpack>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var gatheringTime = ref entity.GetComponent<GatheringTime>().Time;
                ref var gathering = ref entity.GetComponent<Gathering>();
                
                if (gathering.TargetResource.IsNullOrDisposed())
                {
                    FindNewResource(entity);
                    continue;
                }
                
                ref var targetHealth = ref gathering.TargetResource.GetComponent<HealthCurrent>().Value;

                if (targetHealth <= 0)
                {
                    FindNewResource(entity);
                    continue;
                }
                
                gathering.CurrentTime += deltaTime;
                if (gathering.CurrentTime < gatheringTime)
                {
                    continue;
                }
                gathering.CurrentTime = 0;

                ref var backpack = ref entity.GetComponent<UnitBackpack>();
                
                targetHealth -= 1;
                backpack.Amount += 1;
                if (targetHealth <= 0)
                {
                    // TODO: нужно нормально "наносить урон" и уночтижать рес и view
                    Debug.Log("Remove entity resource");
                    gathering.TargetResource.Dispose();

                    if (backpack.Amount < backpack.Capacity)
                    {
                        FindNewResource(entity);
                    }
                }
                
                if (backpack.Amount >= backpack.Capacity)
                {
                    if (!gathering.TargetResource.IsNullOrDisposed())
                    {
                        gathering.TargetResource.RemoveComponent<SomeoneGatheringThis>();
                    }
                    
                    gathering.ProgressEntity.Dispose();
                    entity.RemoveComponent<Gathering>();
                    entity.SetComponent(new FindStorageRequest());
                }
            }
        }

        private static void FindNewResource(in Entity entity)
        {
            entity.GetComponent<Gathering>().ProgressEntity.Dispose();
            entity.RemoveComponent<Gathering>();
            entity.SetComponent(new FindResourceRequest());
        }

        public void Dispose()
        {
        }
    }
}