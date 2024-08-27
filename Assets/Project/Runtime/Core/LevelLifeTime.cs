using Project.Runtime.ECS;
using Project.Runtime.Features;
using Project.Runtime.Features.Building;
using Project.Runtime.Features.BuildingsManagement;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.GameplayMenus;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Project.Runtime.Core
{
    public class LevelLifeTime : ExtendedLifetime
    {
        [SerializeField] private WorldSetup worldSetup;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private MapManager mapManager;
        [Space]
        [SerializeField] private HeaderUI headerUI;
        [SerializeField] private PanelsManager panelsManager;
        [SerializeField] private BuildingManagementPanel buildingManagementPanel;
        [Space]
        [SerializeField] private DayNightCycleEffects dayNightCycleEffects;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<DayNightCycleEffects>(dayNightCycleEffects);
            
            builder.RegisterInstance<PanelsManager>(panelsManager);
            builder.RegisterInstance<BuildingManagementPanel>(buildingManagementPanel);
            
            builder.RegisterInstance<WorldSetup>(worldSetup);
            
            builder.RegisterInstance<CameraController>(cameraController);
            
            builder.RegisterInstance<MapManager>(mapManager);
            
            builder.RegisterInstance<HeaderUI>(headerUI);
            
            // Initialize ecs
            if (World.Default == null) World.Create();
            builder.RegisterInstance<World>(World.Default);
            builder.RegisterEntryPoint<EscBootstrapper>();
        }
    }
}
