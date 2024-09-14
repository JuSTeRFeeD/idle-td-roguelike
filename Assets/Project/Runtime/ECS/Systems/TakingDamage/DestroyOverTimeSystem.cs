using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    public class DestroyOverTimeSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<DestroyOverTime> _destroyOverTimeStash;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<DestroyOverTime>()
                .Build();

            _destroyOverTimeStash = World.GetStash<DestroyOverTime>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var time = ref _destroyOverTimeStash.Get(entity);
                time.EstimateTime -= deltaTime;
                if (time.EstimateTime > 0) continue;
                
                entity.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}