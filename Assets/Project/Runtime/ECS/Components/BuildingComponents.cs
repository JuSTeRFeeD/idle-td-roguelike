using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Project.Runtime.ECS.Components
{
    /// MapResource/Tower
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)] 
    public struct BuildingTag : IComponent
    {
        public int Size;
    }

    /// Tower destroyed, днем нуждается в починке юнитами
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)] 
    public struct BuildingDestroyedTag : IComponent
    {
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct BaseTowerTag : IComponent
    {
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct AttackTowerTag : IComponent
    {
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct LumberjackUnitsOwnedTag : IComponent
    {
        public int CurrentCapacity;
        public MinMaxInt Capacity;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MinerUnitsOwnedTag : IComponent
    {
        public int CurrentCapacity;
        public MinMaxInt Capacity;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct TreeTag : IComponent
    {
    }
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct StoneTag : IComponent
    {
    }

    public interface IStorage : IComponent
    {
        public int Current { get; set; }
        public int Max { get; set; }
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct StoneStorage : IStorage
    {
        public int Current { get; set; }
        public int Max { get; set; }
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct WoodStorage : IStorage
    {
        public int Current { get; set; }
        public int Max { get; set; }
    }

    /// Building placing
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct StartPlaceBuildingCardRequest : IComponent
    {
        public BuildingConfig BuildingConfig;
        public Vector3 StartPlacingPosition;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct PlacingBuildingCard : IComponent
    {
        public BuildingConfig BuildingConfig;
        /// отображение клетки где размещаем объект
        public Entity CellEntity; 
        public Vector3 CurrentPosition;
        public bool IsCollisionDetected;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct PlaceBuildingCardRequest : IComponent
    {
    }
}