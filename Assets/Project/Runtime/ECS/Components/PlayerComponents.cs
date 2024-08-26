using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Components
{
    public struct TotalResourcesData : IComponent
    {
        public int WoodAmount;
        public int WoodCapacity;
        
        public int StoneAmount;
        public int StoneCapacity;
    }

    public struct TotalUnitsData : IComponent
    {
        public int UsedUnitsAmount;
        public int TotalUnitsAmount;
    }
}