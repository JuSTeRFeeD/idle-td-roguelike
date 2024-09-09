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
    public class SpawnFirstBuildingsInitializer : IInitializer
    {
        [Inject] private WorldSetup _worldSetup;
        [Inject] private MapManager _mapManager;

        public World World { get; set; }

        private readonly HashSet<Vector2Int> _excludePoints = new();

        private const int ResourcesToSpawn = 100;
        
        public void OnAwake()
        {
            SpawnBaseTower();
            
            SpawnMapProp(_worldSetup.Rock01, 5);
            
            SpawnResources();
        }

        private void AddExcludePoints(int buildingSize, Vector3 position)
        {
            var gridPos = GridUtils.ConvertWorldToGridPos(position);

            for (var x = 0; x < buildingSize; x++)
            {
                for (var z = 0; z < buildingSize; z++)
                {
                    var pos = new Vector2Int(gridPos.x + x, gridPos.y + z);
                    _excludePoints.Add(pos);
                    
                    foreach (var neighbor in AStarPathfindingSystem.GetNeighbors(pos, _mapManager.MapSize))
                    {
                        _excludePoints.Add(neighbor);
                    }
                }
            }
        }

        private void SpawnBaseTower()
        {
            var request = World.CreateEntity();
            var position = _worldSetup.SpawnBasePoint.position;
            request.SetComponent(new PlacingBuildingCard
            {
                CurrentPosition = position,
                BuildingConfig = _worldSetup.BaseBuildingConfig
            });
            request.SetComponent(new PlaceBuildingCardRequest());
            AddExcludePoints(_worldSetup.BaseBuildingConfig.Size, position);
        }

        private bool BuildingCanBePlacedOnGridPos(Vector2Int gridPos, int buildingSize)
        {
            return gridPos.x - buildingSize > 0 &&
                   gridPos.x + buildingSize < _mapManager.MapSize &&
                   gridPos.y - buildingSize > 0 &&
                   gridPos.y + buildingSize < _mapManager.MapSize;
        }

        private void SpawnMapProp(BuildingConfig propConfig, int count)
        {
            var possiblePositions = _mapManager.Buildings
                .Select(i => i)
                .Where(i => 
                    i.Value is null && 
                    !_excludePoints.Contains(i.Key) &&
                    // AStarPathfindingSystem.GetNeighbors(i.Key, _mapManager.MapSize, propConfig.Size).All(neightbor => neightbor != i.Key) && 
                    BuildingCanBePlacedOnGridPos(i.Key, propConfig.Size)
                )
                .ToList();
            
            for (var i = 0; i < count; i++)
            {
                var idx = Random.Range(0, possiblePositions.Count);
                var position = GridUtils.ConvertGridToWorldPos(possiblePositions[idx].Key);
                possiblePositions.RemoveAt(idx);
                
                var spawnRequest = World.CreateEntity();
                spawnRequest.SetComponent(new PlacingBuildingCard
                {
                    BuildingConfig = propConfig,
                    CurrentPosition = position,
                });
                spawnRequest.SetComponent(new PlaceBuildingCardRequest());
                AddExcludePoints(_worldSetup.Rock01.Size, position);
            }
        }

        private void SpawnResources()
        {
            // Список возможных точек куда можно разместить ресурс
            var possiblePositions = _mapManager.Buildings
                .Select(i => i)
                .Where(i => 
                    i.Value is null && 
                    !_excludePoints.Contains(i.Key) // &&
                    // AStarPathfindingSystem.GetNeighbors(i.Key, _mapManager.MapSize, 1).All(neightbor => neightbor != i.Key)
                )
                .ToList();
            
            for (var i = 0; i < ResourcesToSpawn; i++)
            {
                var idx = Random.Range(0, possiblePositions.Count);
                var cellPos = possiblePositions[idx].Key;
                possiblePositions.RemoveAt(idx);

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
                var idx = Random.Range(0, possiblePositions.Count);
                var cellPos = possiblePositions[idx].Key;
                possiblePositions.RemoveAt(idx);

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