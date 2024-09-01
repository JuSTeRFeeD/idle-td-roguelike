using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Pathfinding
{
    public class AStarMoveSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private const float DistToCompletePath = 0.001f;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<AStarPath>()
                .With<ViewEntity>()
                .With<MoveSpeedRuntime>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var viewTransform = entity.ViewTransform();
                var viewPosition = viewTransform.position;

                ref readonly var moveSpeedRuntime = ref entity.GetComponent<MoveSpeedRuntime>().Value;
                ref var path = ref entity.GetComponent<AStarPath>();
                var diff = path.CurrentTargetPosition - viewPosition;

                var direction = diff;
                    direction.y = 0;
                if (!diff.Equals(Vector3.zero))
                {
                    viewTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                }
                    
                var step = moveSpeedRuntime * deltaTime;
                viewTransform.position = Vector3.MoveTowards(viewPosition, path.CurrentTargetPosition, step);

                if (Vector3.Distance(viewTransform.position, path.CurrentTargetPosition) <= DistToCompletePath)
                {
                    viewTransform.position = path.CurrentTargetPosition;
                    path.CurrentPathIndex++;
                    if (path.CurrentPathIndex >= path.Path.Count)
                    {
                        direction = path.RealTargetPosition - viewTransform.position;
                        direction.y = 0;
                        viewTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                        
                        entity.SetComponent(new MoveToTargetCompleted());
                        entity.RemoveComponent<AStarPath>();
                        continue;
                    }
                    path.CurrentTargetPosition = GridUtils.ConvertGridToWorldPos(path.Path[path.CurrentPathIndex]);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}