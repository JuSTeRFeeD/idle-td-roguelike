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
                .Without<SomeoneGatheringThis>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            var used = new HashSet<Entity>();
            
            foreach (var entity in _unitsFilter)
            {
                var unitPos = entity.GetComponent<ViewEntity>().Value.transform.position;
                var minSqrDis = float.MaxValue;
                var nearestPosition = Vector3.zero;
                Entity nearestEntity = null; 
            
                foreach (var resourceEntity in _mapResourcesFilter)
                {
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
            
                if (nearestEntity == null) return;
            
                entity.SetComponent(new AStarCalculatePathRequest
                {
                    Entity = nearestEntity,
                    TargetPosition = nearestPosition
                });
                entity.SetComponent(new MoveToResource
                {
                    Entity = nearestEntity
                });
                entity.RemoveComponent<FindResourceRequest>();
                nearestEntity.SetComponent(new SomeoneGatheringThis());
                used.Add(nearestEntity);
            }
            
        }

        public void Dispose()
        {
        }
    }
}