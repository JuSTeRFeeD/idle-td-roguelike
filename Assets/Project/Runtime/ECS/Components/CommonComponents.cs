using System;
using NTC.Pool;
using Project.Runtime.ECS.Views;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

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

        public float Evaluate(int curLvl, int maxLvl)
        {
            return min + (max - min) * (curLvl / (float)maxLvl);
        }
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct ToDestroyTag : IComponent
    {
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct DestroyOverTime : IComponent
    {
        public float EstimateTime;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct ViewEntity : IComponent, IDisposable
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
    public struct EntityClickEvent : IComponent
    {
        public Entity Value;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct HealthDefault : IComponent
    {
        public float Value;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct HealthCurrent : IComponent
    {
        public float Value;
        /// Используется вместе с WillDeadAtNextTickTag компонентом
        public float GhostValue;
    }

    /// Это существо умрет в след тике, можно не учитывать в фильтрах некоторых 
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct WillDeadAtNextTickTag : IComponent
    {
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MoveSpeed : IComponent
    {
        public float Value;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct MoveSpeedRuntime : IComponent
    {
        public float Value;
    }
}