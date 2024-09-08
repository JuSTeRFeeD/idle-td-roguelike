using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Enemies
{
    public class EnemyFindTargetSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Filter _buildingsFilter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<ViewEntity>()
                .With<EnemyTag>()
                .Without<AStarPath>()
                .Without<AStarCalculatePathRequest>()
                .Without<AttackTarget>()
                .Build();
            
            _buildingsFilter = World.Filter
                .With<BuildingTag>()
                .With<ViewEntity>()
                .Without<BuildingDestroyedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var entityPos = entity.ViewPosition();
                Entity nearestBuildingEntity = null;
                var nearestPosition = Vector3.zero;
                var minSqrDist = float.MaxValue;
                foreach (var buildingEntity in _buildingsFilter)
                {
                    var pos = buildingEntity.ViewPosition();
                    var sqrDist = Vector3.SqrMagnitude(pos - entityPos);
                    if (sqrDist > minSqrDist) continue;
                    minSqrDist = sqrDist;
                    nearestBuildingEntity = buildingEntity;
                    nearestPosition = pos;
                }

                if (nearestBuildingEntity == null) continue;
                
                entity.SetComponent(new AStarCalculatePathRequest
                {
                    Entity = nearestBuildingEntity,
                    TargetPosition = nearestPosition
                });
            }
        }

        public void Dispose()
        {
        }
    }
}