using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems
{
    public class AttackCooldownSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<IsAttackOnCooldown> _stash;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<IsAttackOnCooldown>()
                .Build();
            
            _stash = World.GetStash<IsAttackOnCooldown>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var cd = ref _stash.Get(entity);
                cd.EstimateTime -= deltaTime;
                if (cd.EstimateTime > 0) continue;

                entity.RemoveComponent<IsAttackOnCooldown>();
            }
        }

        public void Dispose()
        {
        }
    }
}