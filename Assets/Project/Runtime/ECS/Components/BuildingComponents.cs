using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Components
{
    public struct BuildingTag : IComponent
    {
    }
    
    public struct BaseBuildingTag : IComponent
    {
    }

    public struct LumberjackUnitsOwnedTag : IComponent
    {
        public int CurrentCapacity;
        public MinMaxInt Capacity;
    }
    
    public struct MinerUnitsOwnedTag : IComponent
    {
        public int CurrentCapacity;
        public MinMaxInt Capacity;
    }

    public struct TreeTag : IComponent
    {
    }
    public struct StoneTag : IComponent
    {
    }

    public interface IStorage : IComponent
    {
        public int Current { get; set; }
        public int Max { get; set; }
    }
    
    public struct StoneStorage : IStorage
    {
        public int Current { get; set; }
        public int Max { get; set; }
    }

    public struct WoodStorage : IStorage
    {
        public int Current { get; set; }
        public int Max { get; set; }
    }

    // Building placing
    public struct StartPlaceBuildingCardRequest : IComponent
    {
        public BuildingConfig BuildingConfig;
        public Vector3 StartPlacingPosition;
    }
    public struct PlacingBuildingCard : IComponent
    {
        public BuildingConfig BuildingConfig;
        /// отображение клетки где размещаем объект
        public Entity CellEntity; 
        public Vector3 CurrentPosition;
        public bool IsCollisionDetected;
    }
    public struct PlaceBuildingCardRequest : IComponent
    {
    }

    public enum BuildingType
    {
        BaseTower = 0,
        TreeResource = 1,
        StoneResource = 2,
        AttackTower = 3,
    }
}