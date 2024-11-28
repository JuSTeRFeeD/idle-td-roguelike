using Project.Runtime.ECS;
using Project.Runtime.Features;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.BuildingsManagement;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.Inventory;
using Project.Runtime.Features.Leveling;
using Runtime.Features.Tutorial;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Runtime.Core
{
    public class LevelLifeTime : ExtendedLifetime
    {
        [Header("World setup")]
        [SerializeField] private WorldSetup worldSetup;
        [SerializeField] private VfxSetup vfxSetup;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private MapManager mapManager;
        
        [Header("UI")]
        [SerializeField] private PanelsManager panelsManager;
        [SerializeField] private BuildingManagementPanel buildingManagementPanel;
        [SerializeField] private HeaderUI headerUI;
        [SerializeField] private LevelUpPanel levelUpPanel;
        [SerializeField] private GameFinishedPanel gameFinishedPanel;
        
        [Header("Inventory")]
        [SerializeField] private HandsManager handsManager;
        
        [Header("Effects")]
        [SerializeField] private DayNightCycleEffects dayNightCycleEffects;

        [Header("Tutorial")]
        [SerializeField] private TutorialPanel tutorialPanel;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<DayNightCycleEffects>(dayNightCycleEffects);
            
            builder.RegisterInstance<PanelsManager>(panelsManager);
            builder.RegisterInstance<BuildingManagementPanel>(buildingManagementPanel);
            builder.RegisterInstance<HeaderUI>(headerUI);
            builder.RegisterInstance<LevelUpPanel>(levelUpPanel);
            builder.RegisterInstance<GameFinishedPanel>(gameFinishedPanel);
            
            builder.Register<LevelUpCardsManager>(Lifetime.Singleton);
            builder.Register<InventoryStorage>(Lifetime.Singleton);
            builder.RegisterInstance<HandsManager>(handsManager);
            
            builder.RegisterInstance<VfxSetup>(vfxSetup);
            builder.RegisterInstance<WorldSetup>(worldSetup);
            builder.RegisterInstance<CameraController>(cameraController);
            builder.RegisterInstance<MapManager>(mapManager);
            
            builder.RegisterInstance<TutorialPanel>(tutorialPanel);
            
            // Initialize ecs
            if (World.Default == null) World.Create("Main");
            builder.RegisterInstance<World>(World.Default);
            builder.RegisterEntryPoint<EscBootstrapper>();
        }
    }
}
