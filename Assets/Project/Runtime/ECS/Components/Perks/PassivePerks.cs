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
    public struct CannonTowerPerkUpgrades : IComponent
    {
        public float AttackDamageMultiplier;
        public float AttackSpeedMultiplier;
        public float SplashDamageMultiplier;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CrossbowTowerPerkUpgrades : IComponent
    {
        public float AttackDamageMultiplier;
        public float AttackSpeedMultiplier;
    }
}