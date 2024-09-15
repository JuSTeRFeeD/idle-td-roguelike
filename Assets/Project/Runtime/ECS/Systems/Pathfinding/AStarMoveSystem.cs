using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Pathfinding
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class AStarMoveSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<AStarPath> _pathStash;
        private Stash<ViewEntity> _viewStash;
        private Stash<MoveSpeedRuntime> _speedStash;
        
        private const float DistToCompletePath = 0.001f;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<AStarPath>()
                .With<ViewEntity>()
                .With<MoveSpeedRuntime>()
                .Build();
            
            _pathStash = World.GetStash<AStarPath>();
            _viewStash = World.GetStash<ViewEntity>();
            _speedStash = World.GetStash<MoveSpeedRuntime>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var viewTransform = _viewStash.Get(entity).Value.transform;
                var viewPosition = viewTransform.position;

                ref readonly var moveSpeedRuntime = ref _speedStash.Get(entity).Value;
                ref var path = ref _pathStash.Get(entity);
                var diff = path.CurrentTargetPosition - viewPosition;

                var direction = diff;
                    direction.y = 0;
                if (!diff.Equals(Vector3.zero))
                {
                    viewTransform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                }
                    
                var step = moveSpeedRuntime * deltaTime;
                viewTransform.position = Vector3.MoveTowards(viewPosition, path.CurrentTargetPosition, step);

                if (Vector3.SqrMagnitude(path.CurrentTargetPosition - viewTransform.position) <= DistToCompletePath * DistToCompletePath)
                {
                    viewTransform.position = path.CurrentTargetPosition;
                    path.CurrentPathIndex++;
                    
                    // Destination reached
                    if (path.CurrentPathIndex >= path.Path.Count)
                    {
                        direction = path.RealTargetPosition - viewTransform.position;
                        direction.y = 0;
                        viewTransform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                        
                        entity.SetComponent(new MoveToTargetCompleted());
                        entity.RemoveComponent<AStarPath>();
                        continue;
                    }
                    
                    // Last point
                    if (path.CurrentPathIndex == path.Path.Count - 1)
                    {
                        var closeLastPointPos = CloseLastPointPos(viewPosition, path);
                        path.CurrentTargetPosition = closeLastPointPos;
                    }
                    else
                    {
                        path.CurrentTargetPosition = GridUtils.ConvertGridToWorldPos(path.Path[path.CurrentPathIndex]);
                    }
                }
            }
        }

        private static Vector3 CloseLastPointPos(Vector3 viewPosition, AStarPath path)
        {
            var lastPointPos = GridUtils.ConvertGridToWorldPos(path.Path[^1]);
            var closeLastPointPos = lastPointPos + (path.RealTargetPosition - viewPosition).normalized * .4f;
            return closeLastPointPos;
        }

        public void Dispose()
        {
        }
    }
}