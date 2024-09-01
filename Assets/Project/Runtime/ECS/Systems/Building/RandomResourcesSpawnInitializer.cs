using System.Linq;
using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    public class RandomResourcesSpawnInitializer : IInitializer
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private MapManager _mapManager;
        
        public World World { get; set; }

        public void OnAwake()
        {
            var empty = _mapManager.Buildings
                .Select(i => i)
                .Where(i => i.Value is null).ToList();

            for (var i = 0; i < 75; i++)
            {
                var idx = Random.Range(0, empty.Count);
                var cellPos = empty[idx].Key;
                empty.RemoveAt(idx);

                var spawnRequest = World.CreateEntity();
                spawnRequest.SetComponent(new PlacingBuildingCard
                {
                    BuildingConfig = _worldSetup.TreeConfig,
                    CurrentPosition = GridUtils.ConvertGridToWorldPos(cellPos)
                });
                spawnRequest.SetComponent(new PlaceBuildingCardRequest());
            }
            
            for (var i = 0; i < 75; i++)
            {
                var idx = Random.Range(0, empty.Count);
                var cellPos = empty[idx].Key;
                empty.RemoveAt(idx);

                var spawnRequest = World.CreateEntity();
                spawnRequest.SetComponent(new PlacingBuildingCard
                {
                    BuildingConfig = _worldSetup.StoneConfig,
                    CurrentPosition = GridUtils.ConvertGridToWorldPos(cellPos)
                });
                spawnRequest.SetComponent(new PlaceBuildingCardRequest());
            }
        }

        public void Dispose()
        {
        }
    }
}