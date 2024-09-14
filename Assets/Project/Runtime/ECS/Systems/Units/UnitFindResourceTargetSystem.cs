using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitFindResourceTargetSystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitsFilter;
        private Filter _mapResourcesFilter;
        
        private Filter _woodStorageFilter;
        private Filter _stoneStorageFilter;
        
        public void OnAwake()
        {
            _unitsFilter = World.Filter
                .With<UnitTag>()
                .With<FindResourceRequest>()
                .With<ViewEntity>()
                .With<UnitBackpack>()
                .Without<MoveToResource>()
                .Build();
            
            _mapResourcesFilter = World.Filter
                .With<MapResourceTag>()
                .With<ViewEntity>()
                .Without<SomeUnitInteractsWithThisTag>()
                .Build();

            _woodStorageFilter = World.Filter.With<WoodStorage>().Without<WoodStorageFullTag>().Build();
            _stoneStorageFilter = World.Filter.With<StoneStorage>().Without<StoneStorageFullTag>().Build();
        }

        public void OnUpdate(float deltaTime)
        {
            var used = new HashSet<Entity>();

            var needWood = _woodStorageFilter.IsNotEmpty();
            var needStone = _stoneStorageFilter.IsNotEmpty();
            
            if (!needWood && !needStone)
            {
                foreach (var entity in _unitsFilter)
                {
                    entity.RemoveComponent<FindResourceRequest>();
                }
                return;
            }
            
            foreach (var entity in _unitsFilter)
            {
                var unitPos = entity.GetComponent<ViewEntity>().Value.transform.position;
                var minSqrDis = float.MaxValue;
                var nearestPosition = Vector3.zero;
                Entity nearestEntity = null; 
            
                foreach (var resourceEntity in _mapResourcesFilter)
                {
                    if (!needWood && resourceEntity.Has<TreeTag>()) continue;
                    if (!needStone && resourceEntity.Has<StoneTag>()) continue;
                    
                    var pos = resourceEntity.ViewPosition();

                    var sqrDist = Vector3.SqrMagnitude(pos - unitPos);
                    if (sqrDist > minSqrDis || used.Contains(resourceEntity))
                    {
                        continue;
                    }

                    minSqrDis = sqrDist;
                    nearestEntity = resourceEntity;
                    nearestPosition = pos;
                }

                entity.RemoveComponent<FindResourceRequest>();
                if (nearestEntity == null)
                {
                    continue;
                }
            
                entity.SetComponent(new AStarCalculatePathRequest
                {
                    Entity = nearestEntity,
                    TargetPosition = nearestPosition
                });
                entity.SetComponent(new MoveToResource
                {
                    Entity = nearestEntity
                });
                nearestEntity.SetComponent(new SomeUnitInteractsWithThisTag());
                used.Add(nearestEntity);
            }
            
        }

        public void Dispose()
        {
        }
    }
}