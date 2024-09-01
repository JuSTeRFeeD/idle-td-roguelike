using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitMoveToTargetResourceSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<UnitTag>()
                .With<MoveSpeed>()
                .With<MoveToResource>()
                .Without<Gathering>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var moveSpeed = entity.GetComponent<MoveSpeed>().Value;
                var transform = entity.GetComponent<ViewEntity>().Value.transform;

                ref var targetResource = ref entity.GetComponent<MoveToResource>();
                if (targetResource.Entity.IsNullOrDisposed())
                {
                    entity.RemoveComponent<MoveToResource>();
                    entity.SetComponent(new FindResourceRequest());
                    continue;
                }
                
                var resourcePos = targetResource.Entity.GetComponent<ViewEntity>().Value.transform.position;
                
                var direction = (resourcePos - transform.position).normalized;
                var targetPos = resourcePos - direction;  

                var rot = direction;
                rot.y = 0;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                var step = moveSpeed * deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

                if (Vector3.Distance(transform.position, targetPos) <= 0.001f)
                {
                    transform.position = targetPos;
                    entity.SetComponent(new MoveToTargetCompleted());   
                }
            }
        }

        public void Dispose()
        {
        }
    }
}