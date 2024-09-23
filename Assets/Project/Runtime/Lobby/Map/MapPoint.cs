using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Runtime.Lobby.Map
{
    [Serializable]
    public class MapPoint
    {
        public Vector2Int Position { get; private set; }
        public List<MapPoint> Neighbors { get; private set; }
        public RectTransform PointRectTransform { get; private set; }

        public bool IsCompleted { get; private set; }

        public MapPoint(Vector2Int position, RectTransform pointRectTransform)
        {
            Position = position;
            PointRectTransform = pointRectTransform;
            Neighbors = new List<MapPoint>();
        }

        public void AddNeighbor(MapPoint neighbor)
        {
            if (!Neighbors.Contains(neighbor))
            {
                Neighbors.Add(neighbor);
            }
        }
    }
}