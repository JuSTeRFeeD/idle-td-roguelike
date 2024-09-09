using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Enemies;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems
{
    public class FindAttackTargetByRangeSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Filter _enemiesFilter;
        private Filter _allyBuildingsFilter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<AttackRangeRuntime>()
                .Without<AttackTarget>()
                .Without<DestroyedTag>()
                .Build();

            _enemiesFilter = World.Filter
                .With<EnemyTag>()
                .With<ViewEntity>()
                .Build();
            _allyBuildingsFilter = World.Filter
                .With<BuildingTag>()
                .With<ViewEntity>()
                .Without<DestroyedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                if (entity.Has<BuildingTag>())
                {
                    FindTargetWithFilter(entity, _enemiesFilter);
                    continue;
                }
                if (entity.Has<EnemyTag>())
                {
                    FindTargetWithFilter(entity, _allyBuildingsFilter);
                    continue;
                }
            }
        }

        private void FindTargetWithFilter(in Entity entity, in Filter targetsFilter)
        {
            var attackRange = entity.GetComponent<AttackRangeRuntime>().Value;
            var entityPos = entity.ViewPosition();
            
            var nearestSqrMagnitude = float.MaxValue;
            Entity nearestEntity = null; 
            
            foreach (var target in targetsFilter)
            {
                var targetPos = target.ViewPosition();
                var sqrMagnitude = Vector3.SqrMagnitude(targetPos - entityPos);

                if (sqrMagnitude > nearestSqrMagnitude || sqrMagnitude > attackRange * attackRange)
                {
                    continue;
                }
                
                nearestSqrMagnitude = sqrMagnitude;
                nearestEntity = target;
            }

            if (nearestEntity != null)
            {
                entity.SetComponent(new AttackTarget
                {
                    Value = nearestEntity
                });
            }
        }

        public void Dispose()
        {
        }
    }
}