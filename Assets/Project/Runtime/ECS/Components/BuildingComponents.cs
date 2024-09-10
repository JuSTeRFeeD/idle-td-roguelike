using System;
using NTC.Pool;
using Project.Runtime.ECS.Views;
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
    public struct DestroyedTag : IComponent
    {
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct DestroyedView : IComponent, IDisposable
    {
        public EntityView Value;
        public void Dispose()
        {
            if (Value) NightPool.Despawn(Value);
        }
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
    public struct UnitsOwnedTag : IComponent
    {
        public int CurrentCapacity;
        public MinMaxInt Capacity;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MapResourceTag : IComponent
    {
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
        public string CardConfigId;
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

        /// !null when dragging card from inventory  
        public string CardConfigId;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct PlaceBuildingCardRequest : IComponent
    {
    }
}