using Project.Runtime.ECS.Views;
using Project.Runtime.Scriptable;
using Project.Runtime.Scriptable.Buildings;
using Project.Runtime.Scriptable.Enemies;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS
{
    public class WorldSetup : MonoBehaviour
    {
        [Inject] private SceneSharedData _sceneSharedData;
        
        [Header("Player and World")]
        [SerializeField] private DayNightConfig dayNightConfig;
        [SerializeField] private PlayerLevelsConfig playerLevelsConfig;
        
        [Header("Waves & Enemies")]
        [SerializeField] private NightWavesConfig nightWavesConfig;
        [SerializeField] private Transform[] enemySpawnPoints;

        [Header("Building")] 
        [SerializeField] private EntityView destroyedBuildingView;
        [SerializeField] private PlacingCellView placingCellView;
        [SerializeField] private Transform spawnBasePoint;
        [SerializeField] private BuildingConfig baseBuildingConfig;
        
        [Header("Environment Props")]
        [SerializeField] private BuildingConfig rock01;

        [Header("Views")]
        [SerializeField] private EntityView nullView;
        [SerializeField] private RadiusView radiusView;
        [SerializeField] private PopupTextView popupTextView;
        [SerializeField] private WorldHealthBarView worldHealthBarView;
        [SerializeField] private WorldProgressBarView worldProgressBarView;
        [SerializeField] private WorkerUnitView workerUnitView;
        [SerializeField] private BuildingConfig treeConfig;
        [SerializeField] private BuildingConfig stoneConfig;

        // Player And World
        public DayNightConfig DayNightConfig => dayNightConfig;
        public PlayerLevelsConfig PlayerLevelsConfig => playerLevelsConfig;

        // Waves & Enemies
        public NightWavesConfig NightWavesConfig => _sceneSharedData.NightWavesConfig;
        public Transform[] EnemySpawnPoints => enemySpawnPoints;
        
        // Building
        public EntityView DestroyedBuildingView => destroyedBuildingView;
        public PlacingCellView PlacingCellView => placingCellView;
        public Transform SpawnBasePoint => spawnBasePoint;
        public BuildingConfig BaseBuildingConfig => baseBuildingConfig;

        // Environment props
        public BuildingConfig Rock01 => rock01;
        
        // Views
        public EntityView NullView => nullView;
        public RadiusView RadiusView => radiusView;
        public PopupTextView PopupTextView => popupTextView;
        public WorldHealthBarView WorldHealthBarView => worldHealthBarView;
        public WorldProgressBarView WorldProgressBarView => worldProgressBarView;
        public WorkerUnitView WorkerUnitView => workerUnitView;
        public BuildingConfig TreeConfig => treeConfig;
        public BuildingConfig StoneConfig => stoneConfig;
    }
}