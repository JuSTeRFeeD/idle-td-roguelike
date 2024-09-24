using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Runtime.Lobby.Map
{
    [Serializable]
    public class MapPoint
    {
        public Vector2Int Position { get; private set; }
        public List<MapPoint> Neighbors { get; private set; } = new();
        public List<Vector2Int> NeighborPositions { get; private set; } = new();
        public RectTransform PointRectTransform { get; private set; }
        public MapPointView MapPointView { get; private set; }

        public bool isCompleted;
        public MapPointType PointType { get; private set; }
        public enum MapPointType
        {
            Main,
            Bonus
        }
        
        public MapPoint(Vector2Int position, MapPointType pointType, RectTransform pointRectTransform, MapPointView mapPointView)
        {
            Position = position;
            PointType = pointType;
            PointRectTransform = pointRectTransform;
            MapPointView = mapPointView;
        }

        public void AddNeighbor(MapPoint neighbor)
        {
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
                AddNeighbor(neighbor.Position);
            }
        }
        public void AddNeighbor(Vector2Int neighborPosition)
        {
            if (!NeighborPositions.Contains(neighborPosition))
            {
                NeighborPositions.Add(neighborPosition);
            }
        }
    }
}