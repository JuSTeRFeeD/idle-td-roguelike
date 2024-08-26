using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitFindResourceTargetSystem : ISystem
    {
        public World World { get; set; }

        private Filter _unitLumberjackFilter;
        private Filter _treeFilter;
        
        private Filter _unitMinerFilter;
        private Filter _stoneFilter;
        
        public void OnAwake()
        {
            _unitLumberjackFilter = World.Filter
                .With<UnitTag>()
                .With<LumberjackTag>()
                .With<FindResourceRequest>()
                .With<ViewEntity>()
                .Without<MoveToResource>()
                .Build();
            _treeFilter = World.Filter
                .With<TreeTag>()
                .With<ViewEntity>()
                .Without<SomeoneGatheringThis>()
                .Build();
            
            _unitMinerFilter = World.Filter
                .With<UnitTag>()
                .With<MinerTag>()
                .With<FindResourceRequest>()
                .With<ViewEntity>()
                .Without<MoveToResource>()
                .Build();
            _stoneFilter = World.Filter
                .With<StoneTag>()
                .With<ViewEntity>()
                .Without<SomeoneGatheringThis>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            var used = new HashSet<Entity>();
            FindResource(used, _unitLumberjackFilter, _treeFilter);
            FindResource(used, _unitMinerFilter, _stoneFilter);
        }

        private void FindResource(in ISet<Entity> used, Filter unitFilter, Filter resourceFilter)
        {
            foreach (var unitEntity in unitFilter)
            {
                var unitPos = unitEntity.GetComponent<ViewEntity>().Value.transform.position;
                var minSqrDis = float.MaxValue;
                Entity nearestEntity = null; 
                
                foreach (var resourceEntity in resourceFilter)
                {
                    var pos = resourceEntity.GetComponent<ViewEntity>().Value.transform.position;

                    var sqrDist = Vector3.SqrMagnitude(pos - unitPos);
                    if (sqrDist > minSqrDis || used.Contains(resourceEntity))
                    {
                        continue;
                    }

                    minSqrDis = sqrDist;
                    nearestEntity = resourceEntity;
                }
                
                if (nearestEntity == null) continue;
                
                unitEntity.SetComponent(new MoveToResource
                {
                    Entity = nearestEntity
                });
                unitEntity.RemoveComponent<FindResourceRequest>();
                nearestEntity.SetComponent(new SomeoneGatheringThis());
                used.Add(nearestEntity);
            }
        }

        public void Dispose()
        {
        }
    }
}