using System.Collections.Generic;
using System.Linq;
using System.Text;
using Project.Runtime.ECS;
using Runtime.Lobby.Map;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Lobby.Map
{
    public class LevelMapGenerator : MonoBehaviour
    {
        [Inject] private SceneSharedData _sceneSharedData;

        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private Transform gridParent;
        [SerializeField] private Transform lineParent;
        [Space]
        [SerializeField] private int width = 5;
        [SerializeField] private int height = 3;
        [SerializeField] private float cellSpace = 100f;
        [SerializeField] private float lineSize = 10f;

        [MinValue(0f), MaxValue(1f)] [SerializeField]
        private float chanceToGenerateBranch = 0.6f;

        private MapPoint[,] _grid;

        public readonly Dictionary<Vector2Int, MapPointView> ViewsByPosition = new();
        private readonly List<GameObject> _lines = new();

        private Vector2Int StartPos => new(0, height / 2);
        private Vector2Int EndPos => new(width - 1, height / 2);

        public int Width => width - 1;
        
        public void GenerateMap()
        {
            foreach (var mapPointView in ViewsByPosition)
            {
                Destroy(mapPointView.Value.gameObject);
            }
            ViewsByPosition.Clear();
            foreach (var line in _lines)
            {
                Destroy(line);
            }
            _lines.Clear();
            
            _sceneSharedData.MapPoints.Clear();
            _grid = new MapPoint[width, height];
            GenerateMainPath();
            GenerateBranches();
            ConnectPointsWithLines();
            
            foreach (var mapPointView in ViewsByPosition.Values)
            {
                mapPointView.Impact();
            }
        }

        public void LoadMap(List<MapPoint> mapPoints)
        {
            mapPoints[0].IsCanBeSelected = true;
            foreach (var mapPoint in mapPoints)
            {
                CreatePointView(mapPoint);
                ViewsByPosition[mapPoint.Position].SetCompleted(mapPoint.IsCompleted);
                if (mapPoint.IsCompleted)
                {
                    mapPoint.IsCanBeSelected = true;
                    foreach (var mapPointNeighbor in mapPoint.Neighbors)
                    {
                        mapPointNeighbor.IsCanBeSelected = true;
                    }
                }
            }
            _grid = new MapPoint[width, height];
            ConnectPointsWithLines();
        }

        public void LoadMap(string data)
        {
            _sceneSharedData.MapPoints.Clear();
            _grid = new MapPoint[width, height];
            Deserialize(data);
            _sceneSharedData.MapPoints[0].IsCanBeSelected = true;
            foreach (var mapPoint in _sceneSharedData.MapPoints)
            {
                ViewsByPosition[mapPoint.Position].SetCompleted(mapPoint.IsCompleted);
                if (!mapPoint.IsCompleted) continue;
                mapPoint.IsCanBeSelected = true;
                foreach (var mapPointNeighbor in mapPoint.Neighbors)
                {
                    mapPointNeighbor.IsCanBeSelected = true;
                }
            }
            ConnectPointsWithLines();
        }

        private void GenerateMainPath()
        {
            var startPoint = CreateMapPoint(StartPos, false);
            _sceneSharedData.MapPoints.Add(startPoint);

            var currentPosition = StartPos;
            var previousPoint = startPoint;

            while (currentPosition != EndPos)
            {
                var possibleDirections = new List<Vector2Int>();

                // Вправо (основное направление)
                if (currentPosition.x < width - 1)
                {
                    possibleDirections.Add(Vector2Int.right);

                    // Вверх по диагонали
                    if (currentPosition.y < height - 1 && _grid[currentPosition.x + 1, currentPosition.y + 1] == null &&
                        Random.Range(0, 1f) > 0.4f)
                    {
                        possibleDirections.Add(new Vector2Int(1, 1));
                    }

                    // Вниз по диагонали
                    if (currentPosition.y > 0 && _grid[currentPosition.x + 1, currentPosition.y - 1] == null &&
                        Random.Range(0, 1f) > 0.4f)
                    {
                        possibleDirections.Add(new Vector2Int(1, -1));
                    }
                }

                // Дополнительные направления
                if (currentPosition.y > 0 && _grid[currentPosition.x, currentPosition.y - 1] == null)
                    possibleDirections.Add(Vector2Int.down);
                if (currentPosition.y < height - 1 && _grid[currentPosition.x, currentPosition.y + 1] == null)
                    possibleDirections.Add(Vector2Int.up);

                var chosenDirection = possibleDirections[Random.Range(0, possibleDirections.Count)];
                currentPosition += chosenDirection;

                var newPoint = CreateMapPoint(currentPosition, false);
                previousPoint.AddNeighbor(newPoint);
                _sceneSharedData.MapPoints.Add(newPoint);
                previousPoint = newPoint;
            }
        }

        private void GenerateBranches()
        {
            var newBranches = new List<MapPoint>();

            for (var i = 0; i < _sceneSharedData.MapPoints.Count; i++)
            {
                var cell = _sceneSharedData.MapPoints[i];

                if (cell.Position == StartPos || cell.Position.x == EndPos.x) continue;

                if (Random.value > 1f - chanceToGenerateBranch)
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

            _sceneSharedData.MapPoints.AddRange(newBranches);
        }

        private MapPoint CreateMapPoint(Vector2Int position, bool isBranch)
        {
            var type = GetPointMapPointType(position, isBranch);
            var point = new MapPoint(position, type);
            CreatePointView(point);

            _grid[position.x, position.y] = point;

            return point;
        }

        private MapPoint.MapPointType GetPointMapPointType(Vector2Int position, bool isBranch)
        {
            var type = MapPoint.MapPointType.Common;
            if (position == EndPos) type = MapPoint.MapPointType.Boss;
            else if (isBranch) type = MapPoint.MapPointType.Bonus;
            return type;
        }

        private void CreatePointView(MapPoint mapPoint)
        {
            var pointObject = Instantiate(pointPrefab, gridParent);
            pointObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                mapPoint.Position.x * cellSpace,
                mapPoint.Position.y * cellSpace);
            var pointView = pointObject.GetComponent<MapPointView>();
            ViewsByPosition.Add(mapPoint.Position, pointView);
            pointView.Link(mapPoint);
        }

        private void ConnectPointsWithLines()
        {
            foreach (var point in _sceneSharedData.MapPoints)
            {
                foreach (var neighbor in point.Neighbors)
                {
                    CreateLineBetweenPoints(point, neighbor);
                }
            }
        }

        private void CreateLineBetweenPoints(MapPoint pointA, MapPoint pointB)
        {
            var startPosition = ((RectTransform)ViewsByPosition[pointA.Position].transform).anchoredPosition;
            var endPosition = ((RectTransform)ViewsByPosition[pointB.Position].transform).anchoredPosition;

            var line = Instantiate(linePrefab, lineParent);
            var lineRect = line.GetComponent<RectTransform>();

            var direction = (endPosition - startPosition).normalized;
            var distance = Vector2.Distance(startPosition, endPosition);

            lineRect.anchoredPosition = (startPosition + endPosition) / 2;
            lineRect.sizeDelta = new Vector2(distance, lineSize);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineRect.rotation = Quaternion.Euler(0, 0, angle);
            
            _lines.Add(line);
        }

        public static string Serialize(List<MapPoint> mapPoints)
        {
            var sb = new StringBuilder();
            foreach (var point in mapPoints)
            {
                sb.Append($"{point.Position.x},{point.Position.y}|");

                if (point.NeighborPositions.Count > 0)
                {
                    sb.Append(string.Join(";", point.NeighborPositions.Select(np => $"{np.x},{np.y}")));
                }
                else
                {
                    sb.Append("n");
                }

                sb.Append("|");
                sb.Append(point.IsCompleted ? "1" : "0");
                sb.Append("|");
                sb.Append(((byte)point.PointType).ToString());
                sb.Append("|");
            }
            return sb.ToString().TrimEnd('|');
        }

        private void Deserialize(string mapPointsString)
        {
            var mapPoints = new List<MapPoint>();
            var pointStrings = mapPointsString.Split('|');

            for (var i = 0; i < pointStrings.Length; i += 4)
            {
                var positionData = pointStrings[i].Split(',');
                var pointPosition = new Vector2Int(int.Parse(positionData[0]), int.Parse(positionData[1]));

                var neighborsData = pointStrings[i + 1];
                var isCompleted = pointStrings[i + 2] == "1";
                
                var pointType = MapPoint.MapPointType.Common;
                switch (pointStrings[i + 3])
                {
                    case "0":
                        pointType = MapPoint.MapPointType.Common;
                        break;
                    case "1":
                        pointType = MapPoint.MapPointType.Bonus;
                        break;
                    case "2":
                        pointType = MapPoint.MapPointType.Boss;
                        break;
                }

                var newPoint = new MapPoint(pointPosition, pointType) {IsCompleted = isCompleted};

                if (neighborsData != "n")
                {
                    var neighbors = neighborsData.Split(';');
                    foreach (var neighbor in neighbors)
                    {
                        var neighborData = neighbor.Split(',');
                        var neighborPosition = new Vector2Int(int.Parse(neighborData[0]), int.Parse(neighborData[1]));
                        newPoint.NeighborPositions.Add(neighborPosition);
                    }
                }

                mapPoints.Add(newPoint);
                CreatePointView(newPoint);
            }

            _sceneSharedData.MapPoints = mapPoints;

            foreach (var point in mapPoints)
            {
                foreach (var neighborPosition in point.NeighborPositions)
                {
                    var neighbor = _sceneSharedData.MapPoints.Find(p => p.Position == neighborPosition);
                    point.AddNeighbor(neighbor);
                }
            }
        }
    }
}
