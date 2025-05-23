using System.Collections.Generic;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace Project.Runtime.ECS.Components
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct Owner : IComponent
    {
        public Entity Entity;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct FollowOwner : IComponent
    {
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct SpawnUnitRequest : IComponent
    {
        public Entity ForTowerOwner;
        public Vector3 AtPosition;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct UnitTag : IComponent
    {
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct UnitBackpack : IComponent
    {
        public int Capacity;
        public int WoodAmount;
        public int StoneAmount;
        public bool IsCapacityReached => WoodAmount + StoneAmount >= Capacity;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct GatheringTime : IComponent
    {
        public float Time; // Сколько времени нужно чтобы добыть 1 ресурс
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct Gathering : IComponent
    {
        public float CurrentTime;
        public Entity TargetResource;
        public Entity ProgressEntity;
    }

    /// Кто-то уже собирает этот ресурс, нужн выбрать другой
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)] 
    public struct SomeUnitInteractsWithThisTag : IComponent
    {
    }

    /// Найти ресурс для сбора в зависимости от типа юнита
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)] 
    public struct FindResourceRequest : IComponent
    {
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MoveToResource : IComponent
    {
        public Entity Entity;
    }
    
    /// Типо инвентарь заполнен - нужно найти куда сложить ресы
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)] 
    public struct FindStorageRequest : IComponent
    {
        public ResourceType ResourceType;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MoveToStorage : IComponent
    {
        public Entity Entity;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MoveToTargetCompleted : IComponent
    {
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct AStarCalculatePathRequest : IComponent
    {
        public Entity Entity;
        public Vector3 TargetPosition;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct AStarPath : IComponent
    {
        public int CurrentPathIndex;
        public Vector3 CurrentTargetPosition;
        public Vector3 RealTargetPosition;
        public List<Vector2Int> Path;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct UnitMoveToRepairTower : IComponent
    {
        public Entity Tower;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct UnitRepairingTower : IComponent
    {
        public Entity Tower;
        
        public Entity ProgressEntity;
        public float TowerHealthOnStart;
        public float Progress;
    }
}