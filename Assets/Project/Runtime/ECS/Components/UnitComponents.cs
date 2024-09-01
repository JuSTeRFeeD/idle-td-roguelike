using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.ECS.Components
{
    public struct Owner : IComponent
    {
        public Entity Entity;
    }

    public struct FollowOwner : IComponent
    {
    }
    
    public struct SpawnUnitRequest : IComponent
    {
        public Entity ForTowerOwner;
        public Vector3 AtPosition;
        public UnitType UnitType;
    }

    public enum UnitType
    {
        Lumberjack,
        Miner
    }
    
    public struct UnitTag : IComponent
    {
    }

    public struct WorkerUnitTag : IComponent
    {
    }
    
    public struct LumberjackTag : IComponent
    {
    }
    
    public struct MinerTag : IComponent
    {
    }

    public struct UnitBackpack : IComponent
    {
        public int Capacity;
        public int Amount;
    }
    
    public struct GatheringTime : IComponent
    {
        public float Time; // Сколько времени нужно чтобы добыть 1 ресурс
    }
    
    public struct Gathering : IComponent
    {
        public float CurrentTime;
        public Entity TargetResource;
        public Entity ProgressEntity;
    }

    /// Кто-то уже собирает этот ресурс, нужн выбрать другой 
    public struct SomeoneGatheringThis : IComponent
    {
    }

    /// Найти ресурс для сбора в зависимости от типа юнита 
    public struct FindResourceRequest : IComponent
    {
    }
    public struct MoveToResource : IComponent
    {
        public Entity Entity;
    }
    
    /// Типо инвентарь заполнен - нужно найти куда сложить ресы 
    public struct FindStorageRequest : IComponent
    {
        public UnitType UnitType;
    }
    public struct MoveToStorage : IComponent
    {
        public Entity Entity;
    }
    
    public struct MoveToTargetComplete : IComponent
    {
    }
}