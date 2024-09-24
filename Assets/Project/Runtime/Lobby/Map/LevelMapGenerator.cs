using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [MinValue(0f), MaxValue(1f)] [SerializeField]
        private float chanceToGenerateBranch = 0.6f;

        private MapPoint[,] _grid;
        public List<MapPoint> Path { get; private set; } = new();
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

        public void LoadMap(string data)
        {
            _grid = new MapPoint[width, height];
            Deserialize(data);
            ConnectPointsWithLines();
        }

        private void GenerateMainPath()
        {
            var startPoint = CreateMapPoint(StartPos, false);
            Path.Add(startPoint);

            var currentPosition = StartPos;
            var previousPoint = startPoint;

            while (currentPosition != EndPos)
            {
                var possibleDirections = new List<Vector2Int>();

                // Случайное направление если не верхняя линия
                if (currentPosition.y < height - 1)
                {
                    possibleDirections.Add(Vector2Int.up);

                    // Влево
                    if (currentPosition.x > 0 && _grid[currentPosition.x - 1, currentPosition.y] == null &&
                        Random.Range(0, 1f) > 0.4f)
                    {
                        possibleDirections.Add(Vector2Int.left);
                    }

                    // Вправо
                    if (currentPosition.x < width - 1 && _grid[currentPosition.x + 1, currentPosition.y] == null &&
                        Random.Range(0, 1f) > 0.4f)
                    {
                        possibleDirections.Add(Vector2Int.right);
                    }
                }
                else // Только в сторону end точки
                {
                    if (EndPos.x < currentPosition.x) possibleDirections.Add(Vector2Int.left); // Влево  
                    if (EndPos.x > currentPosition.x) possibleDirections.Add(Vector2Int.right); // Вправо 
                }

                var chosenDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
                currentPosition += chosenDirection;

                var newPoint = CreateMapPoint(currentPosition, false);
                previousPoint.AddNeighbor(newPoint);
                Path.Add(newPoint);
                previousPoint = newPoint;
            }
        }
        
        // Генерация ответвлений
        private void GenerateBranches()
        {
            var newBranches = new List<MapPoint>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < Path.Count; i++)
            {
                var cell = Path[i];

                // Не нужно генерировать ответвления на старте и конце
                if (cell.Position == StartPos || cell.Position == EndPos) continue;

                if (Random.value > 1f - chanceToGenerateBranch) // Шанс на создание ответвления
                {
                    var possibleBranches = new List<Vector2Int>();

                    if (cell.Position.x > 0 && _grid[cell.Position.x - 1, cell.Position.y] == null)
                        possibleBranches.Add(Vector2Int.left);
                    if (cell.Position.x < width - 1 && _grid[cell.Position.x + 1, cell.Position.y] == null)
                        possibleBranches.Add(Vector2Int.right);
                    if (cell.Position.y > 0 && _grid[cell.Position.x, cell.Position.y - 1] == null)
                        possibleBranches.Add(Vector2Int.down);
                    if (cell.Position.y < height - 1 && _grid[cell.Position.x, cell.Position.y + 1] == null)
                        possibleBranches.Add(Vector2Int.up);

                    if (possibleBranches.Count <= 0) continue;

                    var branchDirection = possibleBranches[Random.Range(0, possibleBranches.Count)];
                    var branchCellPos = cell.Position + branchDirection;

                    var branchCell = CreateMapPoint(branchCellPos, true);
                    cell.AddNeighbor(branchCell);
                    newBranches.Add(branchCell);
                }
            }

            Path.AddRange(newBranches);
        }

        // Создание объекта MapPoint и его отображения в UI
        private MapPoint CreateMapPoint(Vector2Int position, bool isBranch)
        {
            var pointObject = Instantiate(pointPrefab, gridParent);
            pointObject.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(position.x * cellSpace, position.y * cellSpace);
            var pointView = pointObject.GetComponent<MapPointView>(); 
            var point = new MapPoint(
                position,
                isBranch ? MapPoint.MapPointType.Bonus : MapPoint.MapPointType.Main,
                pointObject.GetComponent<RectTransform>(),
                pointView
                
            );
            _grid[position.x, position.y] = point;

            _cellObjects.Add(pointObject);

            return point;
        }

        // Соединение точек линиями
        private void ConnectPointsWithLines()
        {
            foreach (var point in Path)
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
            lineRect.sizeDelta = new Vector2(distance, lineSize);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineRect.rotation = Quaternion.Euler(0, 0, angle);
        }

        public string Serialize()
        {
            var sb = new StringBuilder();
            foreach (var point in Path)
            {
                sb.Append($"{point.Position.x},{point.Position.y}|"); // Сериализация позиции
        
                // Сериализуем соседей, если они есть
                if (point.NeighborPositions.Count > 0)
                {
                    sb.Append(string.Join(";", point.NeighborPositions.Select(np => $"{np.x},{np.y}")));
                }
                else
                {
                    sb.Append("n"); // 'n' укажет, что нет соседей
                }

                sb.Append("|");
                sb.Append(point.isCompleted ? "1" : "0"); // Статус завершенности
                sb.Append("|");
                sb.Append(point.PointType == MapPoint.MapPointType.Main ? "0" : "1"); // Main (0) / Bonus (1)
                sb.Append("|");
            }
            return sb.ToString().TrimEnd('|'); // Убираем последний символ |
        }


        private void Deserialize(string mapPointsString)
        {
            var mapPoints = new List<MapPoint>();
            var pointStrings = mapPointsString.Split('|');
    
            for (var i = 0; i < pointStrings.Length; i += 4) // Обрабатываем по 4 элемента на точку (позиция, соседи, завершение, тип)
            {
                // Позиция
                var positionParts = pointStrings[i].Split(',');
                var position = new Vector2Int(int.Parse(positionParts[0]), int.Parse(positionParts[1]));

                // Соседи
                var neighborPositions = new List<Vector2Int>();
                if (pointStrings[i + 1] != "n")
                {
                    var neighbors = pointStrings[i + 1].Split(';');
                    foreach (var neighbor in neighbors)
                    {
                        var neighborParts = neighbor.Split(',');
                        var neighborPosition = new Vector2Int(int.Parse(neighborParts[0]), int.Parse(neighborParts[1]));
                        neighborPositions.Add(neighborPosition);
                    }
                }

                // Статус завершенности
                var isCompleted = pointStrings[i + 2] == "1";

                // Тип точки (Main или Bonus)
                var type = pointStrings[i + 3] == "0" ? MapPoint.MapPointType.Main : MapPoint.MapPointType.Bonus;

                // Создаем MapPoint
                var mapPoint = CreateMapPoint(position, type == MapPoint.MapPointType.Bonus);
                mapPoint.isCompleted = isCompleted;
                mapPoint.MapPointView.SetCompleted(isCompleted);
                mapPoint.NeighborPositions.AddRange(neighborPositions);

                mapPoints.Add(mapPoint);
            }

            // Заполняем grid и настраиваем соседей
            foreach (var point in mapPoints)
            {
                _grid[point.Position.x, point.Position.y] = point;
                foreach (var neighborPosition in point.NeighborPositions)
                {
                    point.AddNeighbor(mapPoints.First(i => i.Position == neighborPosition));
                }
            }

            Path = mapPoints;
        }

    }
}