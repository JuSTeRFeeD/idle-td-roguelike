using System.Runtime.CompilerServices;
using NTC.Pool;
using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    public class DeathSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<HealthCurrent>()
                .With<ToDestroyTag>()
                .Without<DestroyedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                // No need dispose towers
                if (entity.Has<BuildingTag>())
                {
                    entity.RemoveComponent<ToDestroyTag>();
                    entity.SetComponent(new DestroyedTag());
                    SpawnDestroyedBuildingView(entity);
                    continue;
                }
                
                // destroy healthbar
                if (entity.Has<HealthbarEntityRef>()) 
                {
                    entity.GetComponent<HealthbarEntityRef>().Value.Dispose();
                }
                entity.Dispose();
            }
        }

        // Disable/Hide real view & spawn destroyed view
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SpawnDestroyedBuildingView(Entity entity)
        {
            ref var view = ref entity.GetComponent<ViewEntity>().Value;
            var viewTransform = view.transform;
            
            view.gameObject.SetActive(false);
            var destroyedView = NightPool.Spawn(
                _worldSetup.DestroyedBuildingView, 
                viewTransform.position,
                viewTransform.rotation);
            entity.SetComponent(new DestroyedView
            {
                Value = destroyedView
            });
        }

        public void Dispose()
        {
        }
    }
}