using System;
using System.Collections.Generic;
using DG.Tweening;
using NTC.Pool;
using Project.Runtime.ECS.Views;
using Project.Runtime.Features.Building.Data;
using Project.Runtime.Scriptable.Buildings;
using UnityEngine;

namespace Project.Runtime.Features.Building
{
    public class MapManager : MonoBehaviour
    {
        public int mapSize;

        public readonly Dictionary<Vector2Int, BuildingData> Buildings = new();

        private void Awake()
        {
            for (var x = 0; x < mapSize; x++)
            {
                for (var z = 0; z < mapSize; z++)
                {
                    Buildings.Add(new Vector2Int(x, z), null);
                }
            }
        }

        public EntityView PutBuilding(BuildingConfig buildingConfig, Vector2Int gridPos, Quaternion rotation)
        {
            EntityView viewResult = null;
            for (var x = 0; x < buildingConfig.Size.x; x++)
            {
                for (var z = 0; z < buildingConfig.Size.y; z++)
                {
                    var pos = new Vector2Int(gridPos.x + x, gridPos.y + z);
                    var isRootPos = pos.Equals(gridPos);
                    Buildings[pos] = new BuildingData
                    {
                        id = buildingConfig.uniqueID,
                        gridIdx = ConvertToIndex(pos),
                        rotY = rotation.eulerAngles.y,
                        IsRootPos = isRootPos
                    };
                    if (isRootPos)
                    {
                        var additionalOffset = Vector3.zero;
                        if (buildingConfig.Size != Vector2Int.one)
                        {
                            additionalOffset = new Vector3(
                                buildingConfig.Size.x / 2f * GridUtils.CellHalf,
                                0,
                                buildingConfig.Size.y / 2f * GridUtils.CellHalf);   
                        }
                        viewResult = NightPool.Spawn(
                            buildingConfig.Prefab,
                            GridUtils.ConvertGridToWorldPos(pos) + additionalOffset,
                            rotation);

                        // TODO: animate with scale placed object
                        // viewResult.transform
                            // .DOPunchScale(Vector3.up * .25f, 0.25f, 10, 2f)
                            // .SetLink(viewResult.gameObject);
                    }
                }
            }

            // if (!isInitialized)
            // {
                // return;
            // }
            
            // Save();

            return viewResult;
        }
        
        private int ConvertToIndex(Vector2Int coordinates)
        {
            var x = coordinates.x;
            var y = coordinates.y;

            if (x < 0 || x >= mapSize || y < 0 || y >= mapSize)
            {
                Debug.LogError("Coordinates are out of bounds!");
                return -1;
            }

            return y * mapSize + x;
        }
        
        private Vector2Int ConvertToGirdPos(int gridIndex)
        {
            if (gridIndex < 0 || gridIndex >= mapSize * mapSize)
            {
                // return new Vector2Int(-1, -1);
                throw new Exception($"Out of bounds grid index {gridIndex}");
            }

            return new Vector2Int(gridIndex / mapSize, gridIndex % mapSize);
        }
        
        /// <summary>
        /// Проверяем, заняты ли клетки в области размещения постройки
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posZ"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>True - клетки заняты</returns>
        public bool CheckCollision(int posX, int posZ, int width, int height)
        {
            // out of bounds
            if (posX < 0 || posX > mapSize || 
                posX + width > mapSize || 
                posZ < 0 || posZ > mapSize ||
                posZ + height > mapSize) return false;
            
            for (var x = 0; x < width; x++)
            {
                for (var z = 0; z < height; z++)
                {
                    if (Buildings[new Vector2Int(posX + x, posZ + z)] != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        /// <summary>
        /// Занята ли клетка
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posZ"></param>
        /// <returns>True - клетка занята</returns>
        public bool CheckCollision(int posX, int posZ)
        {
            // out of bounds
            if (posX < 0 || posX > mapSize || 
                posZ < 0 || posZ > mapSize) return false;
            
            return Buildings[new Vector2Int(posX, posZ)] != null;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            for (var x = 0; x < mapSize; x++)
            {
                for (var z = 0; z < mapSize; z++)
                {
                    // + (x % 2 == 0 ? 0 : GridUtils.CellSize / 2f)
                    Gizmos.DrawWireCube(
                        new Vector3(x * GridUtils.CellSize, .01f, z * GridUtils.CellSize) + new Vector3(GridUtils.CellHalf, 0, GridUtils.CellHalf),
                        new Vector3(GridUtils.CellSize, 0, GridUtils.CellSize)
                        );
                }
            }
        }
    }
}