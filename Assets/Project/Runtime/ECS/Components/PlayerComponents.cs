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

    public struct PlayerLevel : IComponent
    {
        public float CurrentExp;
        public float TargetExp;
        public int Level;
        public int[] ExpByLevel;
    }

    public struct PlayerAddExp : IComponent
    {
        public float Value;
    }

    public struct LevelUp : IComponent
    {
        public int LevelUpsCount;
    }
    
    public struct IsChoosingLevelUpCard : IComponent {}
}