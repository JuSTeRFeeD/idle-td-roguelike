using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Pathfinding
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class AStarPathfindingSystem : ISystem
    {
        [Inject] private MapManager _mapManager;

        public World World { get; set; }

        private Filter _filter;
        private Filter _mapChangedFilter;

        private readonly Dictionary<(Vector2Int, Vector2Int), List<Vector2Int>> _pathCache = new();
        private float _delay = 0;
        private const float DelayTime = 0.2f;
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<AStarCalculatePathRequest>()
                .With<ViewEntity>()
                .Build();

            _mapChangedFilter = World.Filter
                .With<MapGridChangedOneFrame>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            // When map changed we need to recalculate cached path
            if (_mapChangedFilter.IsNotEmpty())
            {
                UnityEngine.Debug.Log("MAP CHANGED -> RESET CACHE FOR ASTAR");
                _pathCache.Clear();
            }
            
            _delay -= deltaTime;
            if (_delay > 0) return;
            _delay = DelayTime;
            
            foreach (var entity in _filter)
            {
                ref readonly var request = ref entity.GetComponent<AStarCalculatePathRequest>();
                var gridPos = GridUtils.ConvertWorldToGridPos(entity.ViewPosition());
                var targetGridPos = GridUtils.ConvertWorldToGridPos(request.TargetPosition);
                var size = 1;
                if (!request.Entity.IsNullOrDisposed() && request.Entity.Has<BuildingTag>())
                {
                    size = request.Entity.GetComponent<BuildingTag>().Size;
                }

                var result = FindPath(gridPos, targetGridPos, size, true); // entity.Has<UnitTag>());
                if (result == null) continue;
                
                entity.SetComponent(new AStarPath
                {
                    CurrentPathIndex = 0,
                    CurrentTargetPosition = GridUtils.ConvertGridToWorldPos(result[0]),
                    RealTargetPosition = request.TargetPosition,
                    Path = result
                });
                entity.RemoveComponent<AStarCalculatePathRequest>();
            }
        }

        public void Dispose() { }

        private struct PathNode : IComparable<PathNode>
        {
            public Vector2Int Position;
            public int GCost; // Стоимость от начала до текущего узла
            public int HCost; // Оценка оставшейся стоимости до цели

            public int FCost => GCost + HCost;

            public PathNode(Vector2Int position, int gCost, int hCost)
            {
                Position = position;
                GCost = gCost;
                HCost = hCost;
            }
            
            public int CompareTo(PathNode other)
            {
                int compare = FCost.CompareTo(other.FCost);
                // Если FCost равен, сравниваем HCost, чтобы избежать тупиков
                return compare == 0 ? HCost.CompareTo(other.HCost) : compare;
            }
        }

        private static List<Vector2Int> _reusableNeighborsList = new();
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<Vector2Int> GetNeighbors(Vector2Int position, int mapSize, int step = 1)
        {
            _reusableNeighborsList.Clear();
            _reusableNeighborsList = step switch
            {
                3 => new List<Vector2Int>
                {
                    new(position.x - 2, position.y),
                    new(position.x - 2, position.y + 1),
                    new(position.x - 2, position.y + 2),
                    new(position.x - 1, position.y + 2),
                    new(position.x, position.y + 2),
                    new(position.x + 1, position.y + 2),
                    new(position.x + 2, position.y + 2),
                    new(position.x + 2, position.y + 1),
                    new(position.x + 2, position.y),
                    new(position.x + 2, position.y - 1),
                    new(position.x + 2, position.y - 2),
                    new(position.x + 1, position.y - 2),
                    new(position.x, position.y - 2),
                    new(position.x - 1, position.y - 2),
                    new(position.x - 2, position.y - 2),
                },
                2 => new List<Vector2Int>
                {
                    new(position.x - 1, position.y),
                    new(position.x - 1, position.y + 1),
                    new(position.x - 1, position.y + 2),
                    new(position.x, position.y + 2),
                    new(position.x + 1, position.y + 2),
                    new(position.x + 2, position.y + 2),
                    new(position.x + 2, position.y + 1),
                    new(position.x + 2, position.y),
                    new(position.x + 2, position.y - 1),
                    new(position.x + 1, position.y - 1),
                    new(position.x, position.y - 1),
                    new(position.x - 1, position.y - 1),
                },
                _ => new List<Vector2Int>
                {
                    new(position.x - 1, position.y),
                    new(position.x - 1, position.y + 1),
                    new(position.x, position.y + 1),
                    new(position.x + 1, position.y + 1),
                    new(position.x + 1, position.y),
                    new(position.x + 1, position.y - 1),
                    new(position.x, position.y - 1),
                    new(position.x - 1, position.y - 1)
                }
            };

            _reusableNeighborsList.RemoveAll(neighbor =>
                neighbor.x < 0 || neighbor.x >= mapSize || neighbor.y < 0 || neighbor.y >= mapSize);

            return _reusableNeighborsList;
        }

        private List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int targetSize, bool forUnitEntity)
        {
            start = FindNearestAvailableBorderPoint(start, forUnitEntity);

            if (_mapManager.Buildings[end] != null)
            {
                end = FindBestAvailableAdjacentPoint(start, end, targetSize, forUnitEntity);
            }

            // Если путь уже был использован - реюз
            // Кеш не будет работать на часто изменяемой карте..
            if (_pathCache.TryGetValue((start, end), out var cachedPath))
            {
                return cachedPath;
            }
            
            var openSet = new PriorityQueue<PathNode>();
            var closedSet = new HashSet<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var gScores = new Dictionary<Vector2Int, int> { [start] = 0 }; // Словарь для хранения G-стоимостей

            openSet.Enqueue(new PathNode(start, 0, GetHeuristic(start, end)));

            while (openSet.Count > 0)
            {
                var currentNode = openSet.Dequeue();

                if (currentNode.Position == end)
                {
                    var path = ReconstructPath(cameFrom, currentNode.Position);
                    _pathCache[(start, end)] = path;
                    return path;
                }

                closedSet.Add(currentNode.Position);

                foreach (var neighbor in GetNeighbors(currentNode.Position, _mapManager.MapSize))
                {
                    var neighborBuilding = _mapManager.Buildings[neighbor];
                    // Скип в closedSet.Contains и когда не союзное строение и не для юнита маршрут строим, ну и клетка занята
                    if (closedSet.Contains(neighbor) || 
                        (neighborBuilding != null && !(neighborBuilding.IsAllyBuilding && forUnitEntity)))
                    {
                        continue;
                    }

                    var tentativeGScore = currentNode.GCost + 1;

                    if (!gScores.TryGetValue(neighbor, out var neighborGScore) || tentativeGScore < neighborGScore)
                    {
                        cameFrom[neighbor] = currentNode.Position;
                        gScores[neighbor] = tentativeGScore;
                        openSet.Enqueue(new PathNode(neighbor, tentativeGScore, GetHeuristic(neighbor, end)));
                    }
                }
            }

            return null;
        }


        private Vector2Int FindBestAvailableAdjacentPoint(Vector2Int start, Vector2Int occupiedEnd, int targetSize, bool forUnitEntity)
        {
            // находим root индекс
            var occupiedRootPosIdx = _mapManager.Buildings[occupiedEnd].RootIdx; 
            
            // по индексу берем root позицию
            var occupiedRootPosition = _mapManager.ConvertToGrid(occupiedRootPosIdx);
            
            // берем соседние клетки вокруг строение с размером targetSize
            var neighbors = GetNeighbors(occupiedRootPosition, _mapManager.MapSize, targetSize);
            var bestPoint = occupiedEnd;
            var bestDistance = float.MaxValue;

            foreach (var neighbor in neighbors)
            {
                var neighborBuilding = _mapManager.Buildings[neighbor];
                if (neighborBuilding != null && // занята
                    !(neighborBuilding.IsAllyBuilding && forUnitEntity)) // не скипать если строение союзное и идет юнит 
                {
                    continue;
                }
                
                var distance = GetHeuristic(start, neighbor);
                if (distance >= bestDistance) continue;
                
                bestDistance = distance;
                bestPoint = neighbor;
            }

            return bestPoint;
        }

        /// ClampToMap pos and return outsideBestPos if gridCell occupied  
        private Vector2Int FindNearestAvailableBorderPoint(Vector2Int outsidePoint, bool forUnitEntity)
        {
            var bestPoint = ClampToMap(outsidePoint);
            if (_mapManager.Buildings[bestPoint] == null)
            {
                return bestPoint;
            }

            // Ally units can walk throw ally buildings
            if (forUnitEntity && _mapManager.Buildings[bestPoint].IsAllyBuilding)
            {
                return bestPoint;
            }

            var bestDistance = int.MaxValue;

            for (var x = 0; x < _mapManager.MapSize; x++)
            {
                CheckAndUpdateBestPoint(new Vector2Int(x, 0), ref bestPoint, ref bestDistance, outsidePoint);
                CheckAndUpdateBestPoint(new Vector2Int(x, _mapManager.MapSize - 1), ref bestPoint, ref bestDistance, outsidePoint);
            }

            for (var y = 0; y < _mapManager.MapSize; y++)
            {
                CheckAndUpdateBestPoint(new Vector2Int(0, y), ref bestPoint, ref bestDistance, outsidePoint);
                CheckAndUpdateBestPoint(new Vector2Int(_mapManager.MapSize - 1, y), ref bestPoint, ref bestDistance, outsidePoint);
            }

            return bestPoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckAndUpdateBestPoint(Vector2Int point, ref Vector2Int bestPoint, ref int bestDistance, Vector2Int targetPoint)
        {
            if (_mapManager.Buildings[point] != null) return;
            
            var distance = GetHeuristic(targetPoint, point);
            if (distance >= bestDistance) return;
            
            bestDistance = distance;
            bestPoint = point;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector2Int ClampToMap(Vector2Int point)
        {
            point.x = Mathf.Clamp(point.x, 0, _mapManager.MapSize - 1);
            point.y = Mathf.Clamp(point.y, 0, _mapManager.MapSize - 1);
            return point;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetHeuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private static List<Vector2Int> ReconstructPath(IReadOnlyDictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            var totalPath = new List<Vector2Int> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }

            totalPath.Reverse();
            return totalPath;
        }
    }

    public class PriorityQueue<T>
    {
        private readonly List<T> _elements = new();
        private readonly IComparer<T> _comparer = Comparer<T>.Default;

        public int Count => _elements.Count;

        public void Enqueue(T item)
        {
            _elements.Add(item);
            _elements.Sort(_comparer);
        }

        public T Dequeue()
        {
            var bestItem = _elements[0];
            _elements.RemoveAt(0);
            return bestItem;
        }
    }
}
