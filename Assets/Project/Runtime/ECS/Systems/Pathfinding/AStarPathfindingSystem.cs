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
                
                var result = FindPath(gridPos, targetGridPos);
                if (result == null)
                {
                    Debug.Log("Путь НЕ найден");
                    continue;
                }
                
                // if (request.WithoutFirstPoint) result.RemoveAt(0);
                    
                Debug.Log("Путь найден");
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

        private List<Vector2Int> GetNeighbors(Vector2Int position, int mapSize)
        {
            var neighbors = new List<Vector2Int>
            {
                new(position.x - 1, position.y),
                new(position.x + 1, position.y),
                new(position.x, position.y - 1),
                new(position.x, position.y + 1)
            };

            // Отфильтровываем соседей, которые выходят за границы карты
            neighbors.RemoveAll(neighbor => 
                neighbor.x < 0 || 
                neighbor.x >= mapSize ||
                neighbor.y < 0 || 
                neighbor.y >= mapSize);

            return neighbors;
        }

        // Метод для поиска пути от start до end
        private List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
        {
            Debug.Log($"FindPath Start:{start} End:{end} Старт занят: {_mapManager.Buildings[start] != null} EndЗанят: {_mapManager.Buildings[end] != null}");
            
            var openSet = new List<PathNode>();
            var closedSet = new HashSet<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            // Если конечная точка занята, ищем ближайшую незанятую точку
            if (_mapManager.Buildings[end] != null)
            {
                end = FindNearestAvailablePoint(end);
            }
            
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
        
        // Метод для поиска ближайшей доступной точки
        private Vector2Int FindNearestAvailablePoint(Vector2Int occupiedPoint)
        {
            var queue = new Queue<Vector2Int>();
            var visited = new HashSet<Vector2Int>();
            queue.Enqueue(occupiedPoint);

            while (queue.Count > 0)
            {
                var currentPoint = queue.Dequeue();

                // Получаем соседей текущей точки
                foreach (var neighbor in GetNeighbors(currentPoint, _mapManager.MapSize))
                {
                    if (visited.Contains(neighbor))
                        continue;

                    visited.Add(neighbor);

                    // Если соседняя точка не занята, возвращаем её
                    if (_mapManager.Buildings[neighbor] == null)
                    {
                        return neighbor;
                    }

                    // Добавляем соседнюю точку в очередь для дальнейшего исследования
                    queue.Enqueue(neighbor);
                }
            }

            // Если не нашли доступную точку (что маловероятно на реальной карте), возвращаем исходную занятую точку
            return occupiedPoint;
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