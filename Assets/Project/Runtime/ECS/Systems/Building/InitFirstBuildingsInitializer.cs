using System.Collections.Generic;
using System.Linq;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Systems.Pathfinding;
using Project.Runtime.Features.Building;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Building
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class InitFirstBuildingsInitializer : IInitializer
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private MapManager _mapManager;

        public World World { get; set; }

        private readonly HashSet<Vector2Int> _excludePoints = new();

        private const int ResourcesToSpawn = 100;
        
        public void OnAwake()
        {
            InitBaseTower();
            InitResources();
        }

        private void AddExcludePoints(BuildingConfig buildingConfig, Vector3 position)
        {
            var gridPos = GridUtils.ConvertWorldToGridPos(position);

            for (var x = 0; x < buildingConfig.Size; x++)
            {
                for (var z = 0; z < buildingConfig.Size; z++)
                {
                    var pos = new Vector2Int(gridPos.x + x, gridPos.y + z);
                    _excludePoints.Add(pos);
                }
            }
            foreach (var pos in AStarPathfindingSystem.GetNeighbors(gridPos, _mapManager.MapSize, buildingConfig.Size + 1))
            {
                _excludePoints.Add(pos);
            }
        }

        private void InitBaseTower()
        {
            var request = World.CreateEntity();
            var position = _worldSetup.SpawnBasePoint.position;
            request.SetComponent(new PlacingBuildingCard
            {
                CurrentPosition = position,
                BuildingConfig = _worldSetup.BaseBuildingConfig
            });
            request.SetComponent(new PlaceBuildingCardRequest());
            AddExcludePoints(_worldSetup.BaseBuildingConfig, position);
        }

        private void InitResources()
        {
            // Исключаем точки вблизи от построек на карте
            foreach (var (gridPos, building) in _mapManager.Buildings)
            {
                if (building == null) continue;
                
                _excludePoints.Add(gridPos);
                foreach (var posAround in AStarPathfindingSystem.GetNeighbors(gridPos, _mapManager.MapSize))
                {
                    _excludePoints.Add(posAround);
                }
            }
            
            // Список возможных точек куда можно разместить ресурс
            var emptyGridPositions = _mapManager.Buildings
                .Select(i => i)
                .Where(i => i.Value is null && !_excludePoints.Contains(i.Key))
                .ToList();
            
            for (var i = 0; i < ResourcesToSpawn; i++)
            {
                var idx = Random.Range(0, emptyGridPositions.Count);
                var cellPos = emptyGridPositions[idx].Key;
                emptyGridPositions.RemoveAt(idx);

                var spawnRequest = World.CreateEntity();
                spawnRequest.SetComponent(new PlacingBuildingCard
                {
                    BuildingConfig = _worldSetup.TreeConfig,
                    CurrentPosition = GridUtils.ConvertGridToWorldPos(cellPos)
                });
                spawnRequest.SetComponent(new PlaceBuildingCardRequest());
            }
            
            for (var i = 0; i < ResourcesToSpawn; i++)
            {
                var idx = Random.Range(0, emptyGridPositions.Count);
                var cellPos = emptyGridPositions[idx].Key;
                emptyGridPositions.RemoveAt(idx);

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