using Project.Runtime.ECS.Views;
using Project.Runtime.Scriptable;
using Project.Runtime.Scriptable.Buildings;
using Project.Runtime.Scriptable.Card;
using UnityEngine;

namespace Project.Runtime.ECS
{
    public class WorldSetup : MonoBehaviour
    {
        [SerializeField] private DayNightConfig dayNightConfig;
        [SerializeField] private PlayerLevelsConfig playerLevelsConfig;
        [SerializeField] private ActiveCardsListConfig activeCardsListConfig;
        [Space]
        [SerializeField] private PlacingCellView placingCellView;
        [SerializeField] private Transform spawnBasePoint;
        [SerializeField] private BuildingConfig baseBuildingConfig;
        [Space]
        [SerializeField] private WorldProgressBarView worldProgressBarView;
        [SerializeField] private WorkerUnitView unitLumberjack;
        [SerializeField] private WorkerUnitView unitMiner;
        [SerializeField] private BuildingConfig treeConfig;
        [SerializeField] private BuildingConfig stoneConfig;

        public DayNightConfig DayNightConfig => dayNightConfig;
        public PlayerLevelsConfig PlayerLevelsConfig => playerLevelsConfig;
        public ActiveCardsListConfig ActiveCardsListConfig => activeCardsListConfig;

        public PlacingCellView PlacingCellView => placingCellView;
        public Transform SpawnBasePoint => spawnBasePoint;
        public BuildingConfig BaseBuildingConfig => baseBuildingConfig;

        public WorldProgressBarView WorldProgressBarView => worldProgressBarView;
        public WorkerUnitView UnitLumberjack => unitLumberjack;
        public WorkerUnitView UnitMiner => unitMiner;
        public BuildingConfig TreeConfig => treeConfig;
        public BuildingConfig StoneConfig => stoneConfig;
    }
}