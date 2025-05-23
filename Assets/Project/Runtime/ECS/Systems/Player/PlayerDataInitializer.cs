using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Components.Perks;
using Project.Runtime.Features;
using Project.Runtime.Features.CameraControl;
using Scellecs.Morpeh;
using VContainer;

namespace Project.Runtime.ECS.Systems.Player
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class PlayerDataInitializer : IInitializer
    {
        [Inject] private CameraController _cameraController;
        [Inject] private WorldSetup _worldSetup;
        [Inject] private HeaderUI _headerUI;
        
        public World World { get; set; }

        public void OnAwake()
        {
            _cameraController.SetPosition(_worldSetup.SpawnBasePoint.position);
            
            var dataEntity = World.CreateEntity();
            dataEntity.SetComponent(new TotalResourcesData
            {
                StoneAmount = 0,
                WoodAmount = 0
            });
            dataEntity.SetComponent(new TotalUnitsData
            {
                UsedUnitsAmount = 0,
                TotalUnitsAmount = 0
            });

            var expByLevel = _worldSetup.PlayerLevelsConfig.ExpByLevel;
            dataEntity.SetComponent(new PlayerLevel
            {
                Level = 0,
                CurrentExp = 0,
                TargetExp = expByLevel[0],
                ExpByLevel = expByLevel
            });
            
            // Level up to choose first cards
            dataEntity.SetComponent(new LevelUp
            {
                LevelUpsCount = 1
            });
            
            // Just reset ui data on start
            _headerUI.SetLevel(0);
            _headerUI.SetLevelExp(0, 1);
            
            InitTowersUpgradesEntities();
            InitStatisticsEntities();
        }

        private void InitStatisticsEntities()
        {
            var entity = World.CreateEntity();
            entity.AddComponent<StatisticTag>();
            entity.SetComponent(new TotalPlacedTowersStatistic { Value = 0});
            entity.AddComponent<TotalDealtDamageStatistic>();
            entity.AddComponent<TotalKilledEnemiesStatistic>();
            entity.AddComponent<TotalKilledGroundEnemiesStatistic>();
            entity.AddComponent<TotalKilledFlyEnemiesStatistic>();
            entity.AddComponent<TotalKilledBossEnemiesStatistic>();
        }

        private void InitTowersUpgradesEntities()
        {
            // TODO: GET FROM PLAYER PROGRESS
            const float criticalChance = 0.1f;
            const float criticalDamage = 0.2f; 
            
            // Cannon
            var cannonUpgradesEntity = World.CreateEntity();
            cannonUpgradesEntity.AddComponent<CannonTowerUpgradesTag>();
            cannonUpgradesEntity.SetComponent(new TowerWithSplashDamageUpgrades
            {
                AdditionalSplashDamagePercent = 0f,
                AdditionalSplashRadius = 0f
            });
            cannonUpgradesEntity.SetComponent(new TowerAttackUpgrades
            {
                AttackDamageMultiplier = 1,
                AttackSpeedMultiplier = 1,
                CriticalChance = criticalChance,
                CriticalDamage = criticalDamage
            });
            
            // Crossbow
            var crossbowUpgradesEntity = World.CreateEntity();
            crossbowUpgradesEntity.AddComponent<CrossbowTowerUpgradesTag>();
            crossbowUpgradesEntity.SetComponent(new TowerAttackUpgrades
            {
                AttackDamageMultiplier = 1,
                AttackSpeedMultiplier = 1,
                CriticalChance = criticalChance,
                CriticalDamage = criticalDamage
            });
            
            // Crystal
            var crystalUpgradesEntity = World.CreateEntity();
            crystalUpgradesEntity.AddComponent<CrystalTowerUpgradesTag>();
            crystalUpgradesEntity.SetComponent(new TowerAttackUpgrades
            {
                AttackDamageMultiplier = 1,
                AttackSpeedMultiplier = 1,
                CriticalChance = criticalChance,
                CriticalDamage = criticalDamage
            });
            crystalUpgradesEntity.SetComponent(new TowerWithBouncesUpgrade
            {
                AdditionalBounces = 0
            });
            
            // Bomb
            var bombUpgradesEntity = World.CreateEntity();
            bombUpgradesEntity.AddComponent<BombTowerUpgradesTag>();
            bombUpgradesEntity.SetComponent(new DontDestroyBombTowerPerk
            {
                ChanceToDontDestroy = 0.3f
            });
            bombUpgradesEntity.SetComponent(new TowerAttackUpgrades
            {
                AttackDamageMultiplier = 1,
                AttackSpeedMultiplier = 1,
                CriticalChance = criticalChance,
                CriticalDamage = criticalDamage
            });
            
            // Tomb
            var tombUpgradesEntity = World.CreateEntity();
            tombUpgradesEntity.AddComponent<TombTowerUpgradesTag>();
            tombUpgradesEntity.SetComponent(new TowerAttackUpgrades
            {
                AttackDamageMultiplier = 1,
                AttackSpeedMultiplier = 1,
                CriticalChance = criticalChance,
                CriticalDamage = criticalDamage
            });
            
            // Snowman
            var snowmanUpgradesEntity = World.CreateEntity();
            snowmanUpgradesEntity.AddComponent<SnowmanTowerUpgradesTag>();
            snowmanUpgradesEntity.SetComponent(new TowerAttackUpgrades
            {
                AttackDamageMultiplier = 1,
                AttackSpeedMultiplier = 1,
                CriticalChance = criticalChance,
                CriticalDamage = criticalDamage
            });
            
            // Pumpkin
            var pumpkinUpgradesEntity = World.CreateEntity();
            pumpkinUpgradesEntity.AddComponent<PumpkinTowerUpgradesTag>();
            pumpkinUpgradesEntity.SetComponent(new PoisonDustDamageUpgrade
            {
                DustDamageMultiplier = 1,
                DustLifetimeMultiplier = 1,
                DustRadiusMultiplier = 1,
                TimeBetweenDustAttackMultiplier = 1
            });
            pumpkinUpgradesEntity.SetComponent(new TowerAttackUpgrades
            {
                AttackDamageMultiplier = 1,
                AttackSpeedMultiplier = 1,
                CriticalChance = criticalChance,
                CriticalDamage = criticalDamage
            });
        }

        public void Dispose()
        {
        }
    }
}