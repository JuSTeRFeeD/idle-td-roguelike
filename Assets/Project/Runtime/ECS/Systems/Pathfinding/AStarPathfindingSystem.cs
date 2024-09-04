using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Extensions;
using Project.Runtime.Features.Building;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.Pathfinding
{
    public class AStarPathfindingSystem : ISystem
    {
        [Inject] private MapManager _mapManager;

        public World World { get; set; }

        private Filter _filter;

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
                var pos = entity.ViewPosition();
                var gridPos = GridUtils.ConvertWorldToGridPos(pos);
                var targetGridPos = GridUtils.ConvertWorldToGridPos(request.TargetPosition);

                // Печать текущих позиций для отладки
                Debug.Log($"Start Grid Pos: {gridPos}, Target Grid Pos: {targetGridPos}");

                var size = 1;
                if (!request.Entity.IsNullOrDisposed() && request.Entity.Has<BuildingTag>())
                {
                    size = request.Entity.GetComponent<BuildingTag>().Size;
                }
                
                var result = FindPath(gridPos, targetGridPos, size);
                if (result == null)
                {
                    Debug.Log("Path not found");
                    continue;
                }

                // Проверяем, нашёлся ли корректный путь
                Debug.Log($"Path found with length: {result.Count}");

                // Убираем первую точку из пути, если нужно
                // if (request.WithoutFirstPoint) result.RemoveAt(0);

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


        public void Dispose()
        {
        }

        private class PathNode
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
        }

        public static List<Vector2Int> GetNeighbors(Vector2Int position, int mapSize, int step = 1)
        {
            List<Vector2Int> neighbors;

            if (step == 3)
            {
                neighbors = new List<Vector2Int>
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
                };
            }
            else if (step == 2)
            {
                neighbors = new List<Vector2Int>
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
                };
            }
            else
            {
                neighbors = new List<Vector2Int>
                {
                    new(position.x - 1, position.y),
                    new(position.x - 1, position.y + 1),
                    new(position.x, position.y + 1),
                    new(position.x + 1, position.y + 1),
                    new(position.x + 1, position.y),
                    new(position.x + 1, position.y - 1),
                    new(position.x, position.y - 1),
                    new(position.x - 1, position.y - 1)
                };
            }

            // Отфильтровываем соседей, которые выходят за границы карты
            neighbors.RemoveAll(neighbor =>
                neighbor.x < 0 ||
                neighbor.x >= mapSize ||
                neighbor.y < 0 ||
                neighbor.y >= mapSize);

            return neighbors;
        }

        // Метод для поиска пути от start до end
        private List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, int targetSize)
        {
            Debug.Log($"FindPath Start:{start} End:{end}");

            // Обработка случая, когда начальная точка находится за пределами карты
            start = ClampToMap(start);
            if (_mapManager.Buildings[start] != null)
            {
                start = FindNearestAvailableBorderPoint(start);
            }

            // Обработка случая, когда конечная точка недоступна
            if (_mapManager.Buildings[end] != null)
            {
                Debug.Log("Конечная недоступна, FindBestAvailableAdjacentPoint");
                end = FindBestAvailableAdjacentPoint(start, end, targetSize);
            }

            Debug.Log($"Updated Start:{start} End:{end}");
            Debug.Log(
                $"Старт занят: {_mapManager.Buildings[start] != null} End занят: {_mapManager.Buildings[end] != null}");

            var openSet = new List<PathNode>();
            var closedSet = new HashSet<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            openSet.Add(new PathNode(start, 0, GetHeuristic(start, end)));

            while (openSet.Count > 0)
            {
                // Находим узел с минимальной стоимостью
                var currentNode = GetNodeWithLowestCost(openSet);

                if (currentNode.Position == end)
                {
                    // Путь найден, восстанавливаем путь
                    return ReconstructPath(cameFrom, currentNode.Position);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode.Position);

                foreach (var neighbor in GetNeighbors(currentNode.Position, _mapManager.MapSize))
                {
                    if (closedSet.Contains(neighbor) || _mapManager.Buildings[neighbor] != null)
                    {
                        continue;
                    }

                    var tentativeGScore = currentNode.GCost + 1;

                    var neighborNode = openSet.Find(node => node.Position == neighbor);
                    if (neighborNode == null)
                    {
                        openSet.Add(new PathNode(neighbor, tentativeGScore, GetHeuristic(neighbor, end)));
                        cameFrom[neighbor] = currentNode.Position;
                    }
                    else if (tentativeGScore < neighborNode.GCost)
                    {
                        neighborNode.GCost = tentativeGScore;
                        cameFrom[neighbor] = currentNode.Position;
                    }
                }
            }

            // Путь не найден
            return null;
        }

        // Метод для поиска ближайшей доступной точки, прилегающей к конечной
        // TargetSize - по базе (1, 1), но если это тавер к которому нужно подойти, то он может быть (2,2)/(3,3)/..
        private Vector2Int FindBestAvailableAdjacentPoint(Vector2Int start, Vector2Int occupiedEnd,
            in int targetSize)
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

        // Метод для поиска ближайшей доступной точки на границе карты
        private Vector2Int FindNearestAvailableBorderPoint(Vector2Int outsidePoint)
        {
            Vector2Int nearestPoint = ClampToMap(outsidePoint);

            if (_mapManager.Buildings[nearestPoint] == null)
            {
                return nearestPoint;
            }

            int bestDistance = int.MaxValue;
            Vector2Int bestPoint = nearestPoint;

            // Проверка всех клеток на границе карты
            for (int x = 0; x < _mapManager.MapSize; x++)
            {
                CheckAndUpdateBestPoint(new Vector2Int(x, 0), ref bestPoint, ref bestDistance, outsidePoint);
                CheckAndUpdateBestPoint(new Vector2Int(x, _mapManager.MapSize - 1), ref bestPoint, ref bestDistance,
                    outsidePoint);
            }

            for (int y = 0; y < _mapManager.MapSize; y++)
            {
                CheckAndUpdateBestPoint(new Vector2Int(0, y), ref bestPoint, ref bestDistance, outsidePoint);
                CheckAndUpdateBestPoint(new Vector2Int(_mapManager.MapSize - 1, y), ref bestPoint, ref bestDistance,
                    outsidePoint);
            }

            return bestPoint;
        }

        private void CheckAndUpdateBestPoint(Vector2Int point, ref Vector2Int bestPoint, ref int bestDistance,
            Vector2Int targetPoint)
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

        // Метод для сжатия точки к границам карты
        private Vector2Int ClampToMap(Vector2Int point)
        {
            point.x = Mathf.Clamp(point.x, 0, _mapManager.MapSize - 1);
            point.y = Mathf.Clamp(point.y, 0, _mapManager.MapSize - 1);
            return point;
        }

        private int GetHeuristic(Vector2Int a, Vector2Int b)
        {
            // Манхэттенское расстояние (для сетки)
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private PathNode GetNodeWithLowestCost(List<PathNode> openSet)
        {
            var lowestCostNode = openSet[0];

            foreach (var node in openSet)
            {
                if (node.FCost < lowestCostNode.FCost)
                {
                    lowestCostNode = node;
                }
            }

            return lowestCostNode;
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
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
}