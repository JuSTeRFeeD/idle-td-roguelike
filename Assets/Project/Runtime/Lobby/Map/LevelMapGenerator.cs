using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Lobby.Map
{
    public class LevelMapGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private Transform gridParent;
        [SerializeField] private Transform lineParent;
        [Space]
        [SerializeField] private int width = 3;
        [SerializeField] private int height = 5;
        [SerializeField] private float cellSpace = 100f;
        [SerializeField] private float lineSize = 10f;
        [MinValue(0f), MaxValue(1f)]
        [SerializeField] private float chanceToGenerateBranch = 0.6f;

        private MapPoint[,] _grid; 
        private readonly List<MapPoint> _path = new();
        private readonly List<GameObject> _cellObjects = new();

        private Vector2Int StartPos => new(width / 2, 0);
        private Vector2Int EndPos => new(width / 2, height - 1);
        
        public void GenerateMap()
        {
            _grid = new MapPoint[width, height];

            GenerateMainPath();
            GenerateBranches();
            ConnectPointsWithLines();
        }

        private void GenerateMainPath()
        {
            var startPoint = CreateMapPoint(StartPos);
            _path.Add(startPoint);

            var currentPosition = StartPos;
            var previousPoint = startPoint;

            while (currentPosition != EndPos)
            {
                var possibleDirections = new List<Vector2Int>();

                // Вверх
                if (currentPosition.y < height - 1)
                {
                    possibleDirections.Add(Vector2Int.up);
                    
                    // Влево
                    if (currentPosition.x > 0 && _grid[currentPosition.x - 1, currentPosition.y] == null && Random.Range(0, 1f) > 0.4f)  
                    {
                        possibleDirections.Add(Vector2Int.left);         
                    }
                    // Вправо
                    if (currentPosition.x < width - 1 && _grid[currentPosition.x + 1, currentPosition.y] == null && Random.Range(0, 1f) > 0.4f)  
                    {
                        possibleDirections.Add(Vector2Int.right); 
                    }
                }
                else
                {
                    if (EndPos.x < currentPosition.x) possibleDirections.Add(Vector2Int.left); // Влево  
                    if (EndPos.x > currentPosition.x) possibleDirections.Add(Vector2Int.right); // Вправо 
                }

                var chosenDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
                currentPosition += chosenDirection;

                var newPoint = CreateMapPoint(currentPosition);
                previousPoint.AddNeighbor(newPoint);  // Связь с предыдущей точкой
                _path.Add(newPoint);                  // Добавляем в путь
                previousPoint = newPoint;            // Обновляем предыдущую точку
            }
        }


        // Генерация ответвлений
        private void GenerateBranches()
        {
            var newBranches = new List<MapPoint>();

            for (var i = 0; i < _path.Count; i++)
            {
                var cell = _path[i];
                
                // Не нужно генерировать ответвления на старте и конце
                if (cell.Position == StartPos || cell.Position == EndPos) continue;

                if (Random.value > 1f - chanceToGenerateBranch) // Шанс на создание ответвления
                {
                    var possibleBranches = new List<Vector2Int>();

                    if (cell.Position.x > 0 && _grid[cell.Position.x - 1, cell.Position.y] == null) possibleBranches.Add(Vector2Int.left);
                    if (cell.Position.x < width - 1 && _grid[cell.Position.x + 1, cell.Position.y] == null) possibleBranches.Add(Vector2Int.right);
                    if (cell.Position.y > 0 && _grid[cell.Position.x, cell.Position.y - 1] == null) possibleBranches.Add(Vector2Int.down);
                    if (cell.Position.y < height - 1 && _grid[cell.Position.x, cell.Position.y + 1] == null) possibleBranches.Add(Vector2Int.up);

                    if (possibleBranches.Count <= 0) continue;
                    
                    var branchDirection = possibleBranches[Random.Range(0, possibleBranches.Count)];
                    var branchCellPos = cell.Position + branchDirection;

                    // Проверяем, что ответвление только на соседнюю клетку
                    // if (Mathf.Abs(branchCellPos.x - cell.Position.x) <= 1 && Mathf.Abs(branchCellPos.y - cell.Position.y) <= 1)
                    // {
                    var branchCell = CreateMapPoint(branchCellPos);
                    cell.AddNeighbor(branchCell); // Добавляем соседа
                    newBranches.Add(branchCell);
                    // }
                }
            }

            _path.AddRange(newBranches);
        }

        // Создание объекта MapPoint и его отображения в UI
        private MapPoint CreateMapPoint(Vector2Int position)
        {
            var pointObject = Instantiate(pointPrefab, gridParent);
            pointObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(position.x * cellSpace, position.y * cellSpace);
            
            var point = new MapPoint(position, pointObject.GetComponent<RectTransform>());
            _grid[position.x, position.y] = point;

            _cellObjects.Add(pointObject);

            return point;
        }

        // Соединение точек линиями
        private void ConnectPointsWithLines()
        {
            foreach (var point in _path)
            {
                foreach (var neighbor in point.Neighbors)
                {
                    CreateLineBetweenPoints(point, neighbor);
                }
            }
        }

        // Создание линии между двумя точками
        private void CreateLineBetweenPoints(MapPoint pointA, MapPoint pointB)
        {
            var startPosition = pointA.PointRectTransform.anchoredPosition;
            var endPosition = pointB.PointRectTransform.anchoredPosition;

            var line = Instantiate(linePrefab, lineParent);
            var lineRect = line.GetComponent<RectTransform>();

            var direction = (endPosition - startPosition).normalized;
            var distance = Vector2.Distance(startPosition, endPosition);

            lineRect.anchoredPosition = (startPosition + endPosition) / 2;
            lineRect.sizeDelta = new Vector2(distance, 10);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineRect.rotation = Quaternion.Euler(0, 0, angle);
        }

    }
}
