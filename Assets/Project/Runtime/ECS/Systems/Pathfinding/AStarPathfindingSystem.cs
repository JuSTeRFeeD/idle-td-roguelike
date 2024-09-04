using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Pathfinding
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class AStarPathfindingSystem : ISystem
    {
        [Inject] private MapManager _mapManager;

        public World World { get; set; }

        private Filter _filter;

        // private Dictionary<(Vector2Int, Vector2Int), List<Vector2Int>> _pathCache = new();
        
        public void OnAwake()
        {
            _filter = World.Filter
                .With<AStarCalculatePathRequest>()
                .With<ViewEntity>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
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
                
                var result = FindPath(gridPos, targetGridPos, size);
                if (result == null) continue;
                
                entity.RemoveComponent<AStarCalculatePathRequest>();
                entity.SetComponent(new AStarPath
                {
                    CurrentPathIndex = 0,
                    CurrentTargetPosition = GridUtils.ConvertGridToWorldPos(result[0]),
                    RealTargetPosition = request.TargetPosition,
                    Path = result
                });
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

        private List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int targetSize)
        {
            start = FindNearestAvailableBorderPoint(start);

            if (_mapManager.Buildings[end] != null)
            {
                end = FindBestAvailableAdjacentPoint(start, end, targetSize);
            }

            // Если путь уже был использован - реюз
            // Кеш не будет работать на часто изменяемой карте..
            // if (_pathCache.TryGetValue((start, end), out var cachedPath))
            // {
                // return cachedPath;
            // }
            
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
                    // _pathCache[(start, end)] = path;
                    return path;
                }

                closedSet.Add(currentNode.Position);

                foreach (var neighbor in GetNeighbors(currentNode.Position, _mapManager.MapSize))
                {
                    if (closedSet.Contains(neighbor) || _mapManager.Buildings[neighbor] != null)
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


        private Vector2Int FindBestAvailableAdjacentPoint(Vector2Int start, Vector2Int occupiedEnd, int targetSize)
        {
            var neighbors = GetNeighbors(occupiedEnd, _mapManager.MapSize, targetSize);
            Vector2Int bestPoint = occupiedEnd;
            int bestDistance = int.MaxValue;

            foreach (var neighbor in neighbors)
            {
                if (_mapManager.Buildings[neighbor] == null)
                {
                    int distance = GetHeuristic(start, neighbor);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestPoint = neighbor;
                    }
                }
            }

            return bestPoint;
        }

        private Vector2Int FindNearestAvailableBorderPoint(Vector2Int outsidePoint)
        {
            var bestPoint = ClampToMap(outsidePoint);
            if (_mapManager.Buildings[bestPoint] == null)
            {
                return bestPoint;
            }

            int bestDistance = int.MaxValue;

            for (int x = 0; x < _mapManager.MapSize; x++)
            {
                CheckAndUpdateBestPoint(new Vector2Int(x, 0), ref bestPoint, ref bestDistance, outsidePoint);
                CheckAndUpdateBestPoint(new Vector2Int(x, _mapManager.MapSize - 1), ref bestPoint, ref bestDistance, outsidePoint);
            }

            for (int y = 0; y < _mapManager.MapSize; y++)
            {
                CheckAndUpdateBestPoint(new Vector2Int(0, y), ref bestPoint, ref bestDistance, outsidePoint);
                CheckAndUpdateBestPoint(new Vector2Int(_mapManager.MapSize - 1, y), ref bestPoint, ref bestDistance, outsidePoint);
            }

            return bestPoint;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckAndUpdateBestPoint(Vector2Int point, ref Vector2Int bestPoint, ref int bestDistance, Vector2Int targetPoint)
        {
            if (_mapManager.Buildings[point] == null)
            {
                int distance = GetHeuristic(targetPoint, point);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPoint = point;
                }
            }
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
