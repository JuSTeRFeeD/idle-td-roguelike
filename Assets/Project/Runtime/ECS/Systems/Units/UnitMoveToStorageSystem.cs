using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Systems.Units
{
    public class UnitMoveToStorageSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<UnitTag>()
                .With<MoveToStorage>()
                .With<MoveSpeed>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var moveSpeed = entity.GetComponent<MoveSpeed>().Value;
                var transform = entity.GetComponent<ViewEntity>().Value.transform;
                var storagePos = entity.GetComponent<MoveToStorage>().Entity
                    .GetComponent<ViewEntity>().Value.transform.position;
                
                var direction = (storagePos - transform.position).normalized;
                var targetPos = storagePos - direction;

                var rot = direction;
                rot.y = 0;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                var step = moveSpeed * deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

                if (Vector3.Distance(transform.position, targetPos) < 0.001f)
                {
                    transform.position = targetPos;
                    entity.SetComponent(new MoveToTargetComplete());
                }
            }
        }

        public void Dispose()
        {
        }
    }
}