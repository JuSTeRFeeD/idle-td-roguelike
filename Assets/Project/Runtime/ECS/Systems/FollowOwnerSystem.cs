using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems
{
    public class FollowOwnerSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<FollowOwner>()
                .With<Owner>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                entity.ViewTransform().position = entity.Owner().ViewPosition();
            }
        }

        public void Dispose()
        {
        }
    }
}