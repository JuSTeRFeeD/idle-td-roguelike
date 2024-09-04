using System.Runtime.CompilerServices;
using UnityEngine;

namespace Project.Runtime.Features.Building
{
    public static class GridUtils
    {
        public const float CellSize = 1f;
        public const float CellHalf = CellSize / 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Vec3ToCellPos(Vector3 pos)
        {
            pos.y = 0;
            pos.x = Mathf.RoundToInt(pos.x / CellSize) * CellSize + CellHalf;
            pos.z = Mathf.RoundToInt(pos.z / CellSize) * CellSize + CellHalf;
            return pos;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int ConvertWorldToGridPos(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.RoundToInt(pos.x / CellSize), 
                Mathf.RoundToInt(pos.z / CellSize));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ConvertGridToWorldPos(Vector2Int pos)
        {
            return new Vector3(
                pos.x * CellSize,
                0f,
                pos.y* CellSize) + new Vector3(CellHalf - 0.001f, 0, CellHalf - 0.001f);
            // Отнимаем -0.01f чтобы округление в ConvertWorldToGridPos с 0.5 не было вверх
            // Example: Mathf.RoundToInt(59.5) даст 60 и мы выйдем за пределы карты
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 BuildingSizeOffset(int buildingSize = 1)
        {
            if (buildingSize == 1)
            {
                return Vector3.zero;
            }
            
            return new Vector3(
                buildingSize / 2f * CellHalf,
                0,
                buildingSize / 2f * CellHalf);   
        }
    }
}