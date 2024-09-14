using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Units.RepairBuildings
{
    public class FindUnitsToRepairTowersSystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitsFilter;
        private Filter _destroyedBuildingsFilter;
        private Filter _dayTimeFilter;

        public void OnAwake()
        {
            _unitsFilter = World.Filter
                .With<UnitTag>()
                .Without<UnitMoveToRepairTower>()
                .Without<UnitRepairingTower>()
                .Build();
            
            _destroyedBuildingsFilter = World.Filter
                .With<BuildingTag>()
                .With<BuildingDamagedTag>()
                .Without<SomeUnitInteractsWithThisTag>()
                .Build();

            _dayTimeFilter = World.Filter
                .With<IsDayTimeTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_dayTimeFilter.IsEmpty()) return;

            foreach (var buildingEntity in _destroyedBuildingsFilter)
            {
                foreach (var unitEntity in _unitsFilter)
                {
                    // Clearing unit
                    if (unitEntity.Has<Gathering>())
                    {
                        ref var gathering = ref unitEntity.GetComponent<Gathering>();
                        gathering.TargetResource.SafeRemove<SomeUnitInteractsWithThisTag>();
                        gathering.ProgressEntity.Dispose();
                        unitEntity.RemoveComponent<Gathering>();
                    }
                    unitEntity.SafeRemove<AStarPath>();
                    unitEntity.SafeRemove<AStarCalculatePathRequest>();
                    unitEntity.SafeRemove<MoveToResource>();
                    unitEntity.SafeRemove<MoveToStorage>();
                    
                    
                    buildingEntity.SetComponent(new SomeUnitInteractsWithThisTag());
                    unitEntity.SetComponent(new AStarCalculatePathRequest
                    {
                        Entity = buildingEntity,
                        TargetPosition = buildingEntity.ViewPosition()
                    });
                    unitEntity.SetComponent(new UnitMoveToRepairTower
                    {
                        Tower = buildingEntity
                    });

                    // Растягиваем логику на несколько тиков..
                    // Костыль шоб юниты правильно выбирали куда бежать им
                    return;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}