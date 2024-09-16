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
    public struct TowerAttackUpgrades : IComponent
    {
        public float AttackDamageMultiplier;
        public float AttackSpeedMultiplier;
        public float CriticalChance;
        public float CriticalDamage;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CannonTowerUpgradesTag : IComponent
    {
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CannonTowerSplashUpgrade : IComponent
    {
        public float SplashDamageMultiplier;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CrossbowTowerUpgradesTag : IComponent
    {
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CrystalTowerUpgradesTag : IComponent
    {
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct TowerWithBouncesUpgrade : IComponent
    {
        public int AdditionalBounces;
    }
}