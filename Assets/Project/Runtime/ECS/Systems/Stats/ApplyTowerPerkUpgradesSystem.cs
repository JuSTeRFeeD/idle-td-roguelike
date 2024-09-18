using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Stats
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class ApplyTowerPerkUpgradesSystem : ISystem
    {
        public World World { get; set; }

        private Filter _cannonTowerPerkUpgradesFilter;
        private Filter _cannonTowerFilter;
        
        private Filter _crossbowTowerPerkUpgradesFilter;
        private Filter _crossbowTowerFilter;
        
        private Filter _crystalTowerPerkUpgradesFilter;
        private Filter _crystalTowerFilter;
        
        private Stash<AttackDamageRuntime> _attackDamageRuntimeStash;
        private Stash<AttackCooldownRuntime> _attackCooldownRuntimeStash;
        private Stash<TowerAttackUpgrades> _towerAttackUpgradesStash;
        
        public void OnAwake()
        {
            _cannonTowerFilter = World.Filter
                .With<CannonTowerTag>()
                .With<SplashDamageRuntime>()
                .Build();
            _cannonTowerPerkUpgradesFilter = World.Filter
                .With<CannonTowerUpgradesTag>()
                .With<TowerAttackUpgrades>()
                .With<TowerWithSplashDamageUpgrades>()
                .Build();
            
            _crossbowTowerFilter = World.Filter.With<CrossbowTowerTag>().Build();
            _crossbowTowerPerkUpgradesFilter = World.Filter
                .With<CrossbowTowerUpgradesTag>()
                .With<TowerAttackUpgrades>()
                .Build();
            
            _crystalTowerFilter = World.Filter.With<CrystalTowerTag>().Build();
            _crystalTowerPerkUpgradesFilter = World.Filter
                .With<CrystalTowerUpgradesTag>()
                .With<TowerAttackUpgrades>()
                .With<TowerWithBouncesUpgrade>()
                .Build();

            _attackDamageRuntimeStash = World.GetStash<AttackDamageRuntime>();
            _attackCooldownRuntimeStash = World.GetStash<AttackCooldownRuntime>();
            _towerAttackUpgradesStash = World.GetStash<TowerAttackUpgrades>();
        }

        public void OnUpdate(float deltaTime)
        {
            // TowerAttackUpgrades
            ApplyAttackUpgrades(_cannonTowerFilter, _cannonTowerPerkUpgradesFilter);
            ApplyAttackUpgrades(_crossbowTowerFilter, _crossbowTowerPerkUpgradesFilter);
            ApplyAttackUpgrades(_crystalTowerFilter, _crystalTowerPerkUpgradesFilter);
            
            // -- Custom upgrades for towers --
            // Crystal bounces
            foreach (var entity in _crystalTowerPerkUpgradesFilter)
            {
                ref readonly var additionalBounces = ref entity.GetComponent<TowerWithBouncesUpgrade>().AdditionalBounces;
                foreach (var towerEntity in _crystalTowerFilter)
                {
                    towerEntity.GetComponent<TowerWithBouncingProjectileRuntime>().Bounces += additionalBounces;
                }
            }
            // Cannon splash 
            foreach (var entity in _cannonTowerPerkUpgradesFilter)
            {
                ref readonly var splashUpgrades = ref entity.GetComponent<TowerWithSplashDamageUpgrades>();
                foreach (var towerEntity in _cannonTowerFilter)
                {
                    ref var splash = ref towerEntity.GetComponent<SplashDamageRuntime>();
                    splash.Radius += splashUpgrades.AdditionalSplashRadius;
                    splash.PercentFromDamage += splashUpgrades.AdditionalSplashDamagePercent;
                }
            }
        }

        private void ApplyAttackUpgrades(in Filter towerFilter, in Filter upgradesFilter)
        {
            foreach (var entity in upgradesFilter)
            {
                ref readonly var upgrades = ref _towerAttackUpgradesStash.Get(entity);
                foreach (var towerEntity in towerFilter)
                {
                    _attackDamageRuntimeStash.Get(towerEntity).Value *= upgrades.AttackDamageMultiplier;
                    _attackCooldownRuntimeStash.Get(towerEntity).Value *= upgrades.AttackSpeedMultiplier;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}