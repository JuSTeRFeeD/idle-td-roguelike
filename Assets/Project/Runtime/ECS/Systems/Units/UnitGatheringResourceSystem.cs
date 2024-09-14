using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitGatheringResourceSystem : ISystem
    {
        [Inject] private MapManager _mapManager;
        
        public World World { get; set; }

        private Filter _filter;
        private Stash<Gathering> _gatheringStash;
        private Stash<GatheringTime> _gatheringTimeStash;
        private Stash<UnitBackpack> _unitBackpackStash;
        private Stash<HealthCurrent> _healthCurrentStash;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<Gathering>()
                .With<GatheringTime>()
                .With<UnitBackpack>()
                .Build();

            _gatheringStash = World.GetStash<Gathering>();
            _gatheringTimeStash = World.GetStash<GatheringTime>();
            _unitBackpackStash = World.GetStash<UnitBackpack>();
            _healthCurrentStash = World.GetStash<HealthCurrent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref readonly var gatheringTime = ref _gatheringTimeStash.Get(entity).Time;
                ref var gathering = ref _gatheringStash.Get(entity);
                
                if (gathering.TargetResource.IsNullOrDisposed())
                {
                    FindNewResource(entity);
                    continue;
                }

                ref var targetHealth = ref _healthCurrentStash.Get(gathering.TargetResource).Value;

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

                // Adding exp for player
                World.CreateEntity().SetComponent(new PlayerAddExp { Value = 1 });
                
                // Backpack
                ref var backpack = ref _unitBackpackStash.Get(entity);
                
                targetHealth -= 1;
                if (gathering.TargetResource.Has<TreeTag>()) backpack.WoodAmount++;
                if (gathering.TargetResource.Has<StoneTag>()) backpack.StoneAmount++;

                if (targetHealth <= 0)
                {
                    // TODO: нужно "нормально наносить урон" и уночтижать entity с building(MapManager)
                    _mapManager.DestroyBuilding(gathering.TargetResource.ViewPosition());
                    gathering.TargetResource.Dispose();

                    if (!backpack.IsCapacityReached)
                    {
                        FindNewResource(entity);
                        continue;
                    }
                }
                
                if (backpack.IsCapacityReached)
                {
                    if (!gathering.TargetResource.IsNullOrDisposed())
                    {
                        gathering.TargetResource.RemoveComponent<SomeUnitInteractsWithThisTag>();
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