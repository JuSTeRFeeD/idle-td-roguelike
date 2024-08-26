using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Player
{
    public class PlayerDataInitializer : IInitializer
    {
        public World World { get; set; }

        public void OnAwake()
        {
            var dataEntity = World.CreateEntity();
            dataEntity.SetComponent(new TotalResourcesData
            {
                StoneAmount = 0,
                WoodAmount = 0
            });
            dataEntity.SetComponent(new TotalUnitsData()
            {
                UsedUnitsAmount = 0,
                TotalUnitsAmount = 0
            });
        }

        public void Dispose()
        {
        }
    }
}