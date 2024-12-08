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
    public struct CannonTowerUpgradesTag : IComponent { }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct TowerWithSplashDamageUpgrades : IComponent
    {
        public float AdditionalSplashDamagePercent;
        public float AdditionalSplashRadius;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CrossbowTowerUpgradesTag : IComponent { }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct CrystalTowerUpgradesTag : IComponent { }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct TombTowerUpgradesTag : IComponent { }
    
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct BombTowerUpgradesTag : IComponent { }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct PumpkinTowerUpgradesTag : IComponent { }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct SnowmanTowerUpgradesTag : IComponent { }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct TowerWithBouncesUpgrade : IComponent
    {
        public int AdditionalBounces;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct PoisonDustDamageUpgrade : IComponent
    {
        public float DustDamageMultiplier;
        public float TimeBetweenDustAttackMultiplier;
        public float DustLifetimeMultiplier;
        public float DustRadiusMultiplier;
    }

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct DontDestroyBombTowerPerk : IComponent
    {
        public float ChanceToDontDestroy;
    }
}