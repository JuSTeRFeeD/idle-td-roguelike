using UnityEngine;

namespace Project.Runtime.Features.Building
{
    public static class GridUtils
    {
        public const float CellSize = 1f;
        public const float CellHalf = CellSize / 2;

        public static Vector3 Vec3ToCellPos(Vector3 pos)
        {
            pos.y = 0;
            pos.x = Mathf.RoundToInt((pos.x + CellHalf) / CellSize) * CellSize - CellHalf;
            pos.z = Mathf.RoundToInt((pos.z + CellHalf) / CellSize) * CellSize - CellHalf;
            return pos;
        }
        
        public static Vector2Int ConvertWorldToGridPos(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.RoundToInt((pos.x - CellHalf) / CellSize), 
                Mathf.RoundToInt((pos.z - CellHalf) / CellSize));
        }
        
        public static Vector3 ConvertGridToWorldPos(Vector2Int pos)
        {
            return new Vector3(
                pos.x * CellSize,
                0f,
                pos.y* CellSize) + new Vector3(CellHalf, 0, CellHalf);
        }
    }
}