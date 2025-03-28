using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NTC.Pool;
using Project.Runtime.ECS.Components;
using Project.Runtime.ECS.Views;
using Project.Runtime.Features.Building.Data;
using Project.Runtime.Scriptable.Buildings;
using Scellecs.Morpeh;
using UnityEngine;

namespace Project.Runtime.Features.Building
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private int mapSize = 6;

        public int MapSize => mapSize;

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

        public EntityView PutBuilding(in BuildingConfig buildingConfig, Vector2Int gridPos, Quaternion rotation,
            in Entity buildingEntity)
        {
            EntityView viewResult = null;
            for (var x = 0; x < buildingConfig.Size; x++)
            {
                for (var z = 0; z < buildingConfig.Size; z++)
                {
                    var pos = new Vector2Int(gridPos.x + x, gridPos.y + z);
                    var isRootPos = pos.Equals(gridPos);
                    Buildings[pos] = new BuildingData
                    {
                        id = buildingConfig.uniqueID,
                        gridIdx = ConvertToIndex(pos),
                        rotY = rotation.eulerAngles.y,
                        IsRootPos = isRootPos,
                        RootIdx = ConvertToIndex(gridPos),
                        Entity = buildingEntity,
                        IsAllyBuilding = buildingConfig is not MapResourceConfig
                    };
                    if (isRootPos)
                    {
                        viewResult = NightPool.Spawn(
                            buildingConfig.Prefab,
                            GridUtils.ConvertGridToWorldPos(pos) + GridUtils.BuildingSizeOffset(buildingConfig.Size),
                            rotation);
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

        public BuildingData UpgradeBuilding(UpgradableTowerConfig buildingConfig, Vector2Int gridPos)
        {
            var building = Buildings[gridPos];
            
            if (!building.IsRootPos) building = Buildings[ConvertToGrid(building.RootIdx)];
            if (building.id != buildingConfig.uniqueID)
            {
                Debug.LogError($"[MapManager] Id missmatch {building.id} != {buildingConfig.uniqueID} | gridPos {gridPos}");
                return null;
            }
            
            ref var buildingTag = ref building.Entity.GetComponent<BuildingTag>();
            
            // Cant upgrade cuz lvl max
            if (!(buildingTag.Level < buildingConfig.UpgradePrices.Length))
            {
                Debug.LogWarning($"[MapManager] MaxLevel {buildingTag.Level} >= {buildingConfig.UpgradePrices.Length} | gridPos {gridPos}");
                return null;
            }
            
            buildingTag.Level++;
            if (buildingTag.Level == buildingConfig.UpgradePrices.Length)
            {
                building.Entity.AddComponent<MaxLevelReachedTag>();
            }
            
            // Debug.Log($"[MapManager] Merged, new lvl {buildingTag.Level}");
            return building;
        }

        public void DestroyBuilding(Vector3 viewPosition)
        {
            var gridPos = GridUtils.ConvertWorldToGridPos(viewPosition);
            if (Buildings[gridPos].IsRootPos)
            {
                Buildings[gridPos] = null;
                return;
            }
            // TODO: get buildingConfig by id
            // then set Buildings[i] = null for from gridPos to size 
            // Buildings[gridPos].id
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2Int ConvertToGrid(int index)
        {
            if (index < 0 || index >= mapSize * mapSize)
            {
                throw new Exception($"Index {index} is out of bounds");
            }

            var x = index % mapSize;
            var y = index / mapSize;

            return new Vector2Int(x, y);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConvertToIndex(Vector2Int coordinates)
        {
            var x = coordinates.x;
            var y = coordinates.y;

            if (x < 0 || x >= mapSize || y < 0 || y >= mapSize)
            {
                throw new Exception($"Coordinates ${coordinates} are out of bounds");
            }

            return y * mapSize + x;
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
                posZ + height > mapSize) return true;

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
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GridUtils.ConvertGridToWorldPos(GridUtils.ConvertWorldToGridPos(new Vector3(30, 0, 30))) + Vector3.up, 0.5f);
            
            if (Application.isPlaying)
            {
                for (var x = 0; x < mapSize; x++)
                {
                    for (var z = 0; z < mapSize; z++)
                    {
                        if (Buildings[new Vector2Int(x, z)] != null)
                        {
                            Gizmos.color = Color.yellow;
                            Gizmos.DrawCube(
                                new Vector3(x * GridUtils.CellSize, .01f, z * GridUtils.CellSize) +
                                new Vector3(GridUtils.CellHalf, 0, GridUtils.CellHalf),
                                new Vector3(GridUtils.CellSize, 0, GridUtils.CellSize)
                            );
                        }
                        else
                        {
                            Gizmos.color = Color.white;
                            Gizmos.DrawWireCube(
                                new Vector3(x * GridUtils.CellSize, .01f, z * GridUtils.CellSize) +
                                new Vector3(GridUtils.CellHalf, 0, GridUtils.CellHalf),
                                new Vector3(GridUtils.CellSize, 0, GridUtils.CellSize)
                            );
                        }

                        // + (x % 2 == 0 ? 0 : GridUtils.CellSize / 2f)
                    }
                }
            }
            else
            {
                for (var x = 0; x < mapSize; x++)
                {
                    for (var z = 0; z < mapSize; z++)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireCube(
                            new Vector3(x * GridUtils.CellSize, .01f, z * GridUtils.CellSize) +
                            new Vector3(GridUtils.CellHalf, 0, GridUtils.CellHalf),
                            new Vector3(GridUtils.CellSize, 0, GridUtils.CellSize)
                        );
                    }
                }
            }
        }
    }
}