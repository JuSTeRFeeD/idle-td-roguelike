using Project.Runtime.ECS.Views;
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
    public struct StartPlaceBuildingRequest : IComponent
    {
        public BuildingConfig BuildingConfig;
        public Vector3 StartPlacingPosition;
    }
    public struct PlacingBuilding : IComponent
    {
        public BuildingConfig BuildingConfig;
        public EntityView Preview;
        public Vector3 CurrentPosition;
    }
    public struct PlaceBuildingRequest : IComponent
    {
    }

    public enum BuildingType
    {
        Base = 0,
    }
}