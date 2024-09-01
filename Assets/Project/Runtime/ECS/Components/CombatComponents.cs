using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Components
{
    public struct AttackDamage : IComponent
    {
        public float Value;
    }
    
    public struct AttackDamageRuntime : IComponent
    {
        public float Value;
    }
    
    public struct AttackRange : IComponent
    {
        public float Value;
    }
    
    public struct AttackRangeRuntime : IComponent
    {
        public float Value;
    }
    
    public struct AttackCooldown : IComponent
    {
        public float Value;
    }
    
    public struct AttackCooldownRuntime : IComponent
    {
        public float Value;
    }

    public struct EstimateAttackCooldownTime : IComponent
    {
        public float Value;
    }
}