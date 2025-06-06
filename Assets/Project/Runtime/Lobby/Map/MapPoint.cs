using System.Collections.Generic;
using UnityEngine;

namespace Project.Runtime.Lobby.Map
{
    public class MapPoint
    {
        public Vector2Int Position { get; private set; }
        public List<MapPoint> Neighbors { get; private set; } = new();
        public List<Vector2Int> NeighborPositions { get; private set; } = new();

        public bool IsCompleted;
        public bool IsCanBeSelected;
        public MapPointType PointType { get; private set; }
        
        public enum MapPointType
        {
            Common,
            Bonus,
            Boss
        }
        
        public MapPoint(Vector2Int position, MapPointType pointType)
        {
            Position = position;
            PointType = pointType;
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