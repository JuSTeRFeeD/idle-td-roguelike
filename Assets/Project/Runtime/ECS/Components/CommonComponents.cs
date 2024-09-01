using System;
using NTC.Pool;
using Project.Runtime.ECS.Views;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Components
{
    [Serializable]
    public struct MinMaxInt
    {
        public int min;
        public int max;
    }
    
    [Serializable]
    public struct MinMaxFloat
    {
        public float min;
        public float max;
    }
    
    public struct ViewEntity : IComponent, IDisposable
    {
        public EntityView Value;
        
        public void Dispose()
        {
            if (Value) NightPool.Despawn(Value);
        }
    }

    public struct EntityClickEvent : IComponent
    {
        public Entity Value;
    }

    public struct HealthDefault : IComponent
    {
        public float Value;
    }
    
    public struct HealthCurrent : IComponent
    {
        public float Value;
    }

    public struct MoveSpeed : IComponent
    {
        public float Value;
    }
}