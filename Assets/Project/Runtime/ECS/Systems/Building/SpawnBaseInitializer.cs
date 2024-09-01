using Project.Runtime.ECS.Components;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Project.Runtime.ECS.Systems.Building
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class SpawnBaseInitializer : IInitializer
    {
        [Inject] private WorldSetup _worldSetup;

        public World World { get; set; }

        public void OnAwake()
        {
            var request = World.CreateEntity();
            var position = _worldSetup.SpawnBasePoint.position;
            request.SetComponent(new PlacingBuildingCard
            {
                CurrentPosition = position,
                BuildingConfig = _worldSetup.BaseBuildingConfig
            });
            request.SetComponent(new PlaceBuildingCardRequest());
        }

        public void Dispose()
        {
        }
    }
}