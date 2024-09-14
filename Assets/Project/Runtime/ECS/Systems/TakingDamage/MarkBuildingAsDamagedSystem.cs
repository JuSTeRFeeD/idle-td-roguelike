using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.TakingDamage
{
    public class MarkBuildingAsDamagedSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<BuildingTag>()
                .With<DamageAccumulator>()
                .Without<BuildingDamagedTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                entity.SetComponent(new BuildingDamagedTag());
            }
        }

        public void Dispose()
        {
        }
    }
}