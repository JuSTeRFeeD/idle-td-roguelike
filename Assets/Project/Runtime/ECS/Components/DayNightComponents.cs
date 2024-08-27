using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Components
{
    public struct DayNight : IComponent
    {
        public int DayNumber;
        public float EstimateTime;
        public float DayTime;
        public float NightTime;
    }
    
    public struct IsDayTimeTag : IComponent {}
    public struct IsNightTimeTag : IComponent {}
}