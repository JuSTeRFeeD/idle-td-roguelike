using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Scellecs.Morpeh;

namespace Project.Runtime.ECS.Systems.Stats
{
    public class ApplyTowerPerkUpgradesSystem : ISystem
    {
        public World World { get; set; }

        private Filter _cannonPerkUpgradesFilter;
        private Filter _cannonFilter;
        
        private Filter _crossbowPerkUpgradesFilter;
        private Filter _crossbowFilter;
        
        public void OnAwake()
        {
            _cannonPerkUpgradesFilter = World.Filter.With<CannonTowerPerkUpgrades>().Build();
            _cannonFilter = World.Filter.With<CannonTowerTag>().Build();
            
            _crossbowPerkUpgradesFilter = World.Filter.With<CrossbowTowerPerkUpgrades>().Build();
            _crossbowFilter = World.Filter.With<CrossbowTowerTag>().Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // Cannon
            foreach (var perkEntity in _cannonPerkUpgradesFilter)
            {
                ref readonly var perk = ref perkEntity.GetComponent<CannonTowerPerkUpgrades>();
                foreach (var towerEntity in _cannonFilter)
                {
                    towerEntity.GetComponent<AttackDamageRuntime>().Value *= perk.AttackDamageMultiplier;
                    towerEntity.GetComponent<AttackCooldownRuntime>().Value *= perk.AttackSpeedMultiplier;
                }   
            }
            
            // Crossbow
            foreach (var perkEntity in _crossbowPerkUpgradesFilter)
            {
                ref readonly var perk = ref perkEntity.GetComponent<CrossbowTowerPerkUpgrades>();
                foreach (var towerEntity in _crossbowFilter)
                {
                    towerEntity.GetComponent<AttackDamageRuntime>().Value *= perk.AttackDamageMultiplier;
                    towerEntity.GetComponent<AttackCooldownRuntime>().Value *= perk.AttackSpeedMultiplier;
                }   
            }
        }

        public void Dispose()
        {
        }
    }
}