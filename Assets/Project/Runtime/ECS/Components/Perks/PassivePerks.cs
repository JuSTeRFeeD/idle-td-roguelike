using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace Project.Runtime.ECS.Components.Perks
{
    /// Increase for percent gaining experience amount
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct ExpGainIncreasePerk : IComponent
    {
        public float Multiplier;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CannonTowersAttackSpeedMultiplierPerk : IComponent
    {
        public float Value;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct AllTowersAttackSpeedMultiplierPerk : IComponent
    {
        public float Value;
    }
}