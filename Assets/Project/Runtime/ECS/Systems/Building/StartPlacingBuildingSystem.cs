using Project.Runtime.ECS.Components;
using Project.Runtime.Features.CameraControl;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class StartPlacingBuildingSystem : ISystem
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private CameraController _cameraController;
        
        public World World { get; set; }

        private Filter _filter;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<StartPlaceBuildingRequest>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var data = entity.GetComponent<StartPlaceBuildingRequest>();
                
                var building = World.CreateEntity();
                var preview = Object.Instantiate(data.BuildingConfig.Prefab, data.StartPlacingPosition, Quaternion.identity);
                building.SetComponent(new PlacingBuilding
                {
                    CurrentPosition = data.StartPlacingPosition,
                    Preview = preview,
                    BuildingConfig = data.BuildingConfig
                });
                
                _cameraController.OverrideTarget(preview.transform);
                
                entity.Dispose();
            }
        }

        public void Dispose()
        {
        }
    }
}