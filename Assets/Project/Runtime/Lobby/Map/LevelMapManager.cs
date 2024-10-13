using System;
using Project.Runtime.Core;
using Project.Runtime.ECS;
using Project.Runtime.Scriptable.MapLevelConfigs;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Runtime.Lobby.Map;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.Map
{
    public class LevelMapManager : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;
        [Inject] private SceneLoader _sceneLoader;
        [Inject] private SceneSharedData _sceneSharedData;
        [Inject] private MapLevelConfig _mapLevelConfig;
        
        [SerializeField] private LevelMapGenerator levelMapGenerator;
        [SerializeField] private Button startGameButton;

        private MapPointView _selectedView;
        
        private void Start()
        {
            InitMap();
            SubscribeToViews();
            startGameButton.onClick.AddListener(ToTheGame);
        }

        private void ToTheGame()
        {
            // Set enemies config for this run
            var selectedPoint = _sceneSharedData.MapPoints[_persistentPlayerData.CurMapPointIndex];
            var height = _sceneSharedData.MapPoints[_persistentPlayerData.CurMapPointIndex].Position.x;
            if (height == levelMapGenerator.Width)
            {
                _sceneSharedData.NightWavesConfig = _mapLevelConfig.GetBossConfig(_persistentPlayerData.CompletedMapsCount);
            }
            else {
                _sceneSharedData.NightWavesConfig = selectedPoint.PointType switch
                {
                    MapPoint.MapPointType.Common => _mapLevelConfig.GetCommonConfig(_persistentPlayerData.CompletedMapsCount),
                    MapPoint.MapPointType.Bonus => _mapLevelConfig.GetBonusConfig(_persistentPlayerData.CompletedMapsCount),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            
            Debug.Log($"Start game | _sceneSharedData.NightWavesConfig.name: {_sceneSharedData.NightWavesConfig.name}");

            // To the game
            StartCoroutine(_sceneLoader.LoadSceneAsync("Game"));
        }

        private void Save()
        {
            _persistentPlayerData.MapData = LevelMapGenerator.Serialize(_sceneSharedData.MapPoints);
            _saveManager.Save();
        }

        private void InitMap()
        {
            // // cached
            // if (!_sceneSharedData.MapPoints.IsNullOrEmpty())
            // {
            //     levelMapGenerator.LoadMap(_sceneSharedData.MapPoints);
            //     InitSelectedPoint();
            //     return;
            // }
            // // load
            // if (!string.IsNullOrEmpty(_persistentPlayerData.MapData))
            // {
            //     levelMapGenerator.LoadMap(_persistentPlayerData.MapData);
            //     InitSelectedPoint();
            //     return;
            // }
            GenerateNewMap();
        }

        private void InitSelectedPoint()
        {
            var point = _sceneSharedData.MapPoints[_persistentPlayerData.CurMapPointIndex];
            if (point.IsCompleted)
            {
                var notCompletedIndex = _sceneSharedData.MapPoints.FindIndex(i => !i.IsCompleted);
                if (notCompletedIndex == -1)
                {
                    // TODO: Regenerate map with animation
                    Debug.Log("TODO: Regenerate map");
                    return;
                }
                _persistentPlayerData.CurMapPointIndex = notCompletedIndex;
                point = _sceneSharedData.MapPoints[_persistentPlayerData.CurMapPointIndex];
            }
            OnClickPointView(levelMapGenerator._viewsByPosition[point.Position]);
        }

        private void GenerateNewMap()
        {
            levelMapGenerator.GenerateMap();
            _persistentPlayerData.CurMapPointIndex = 0;
            var point = _sceneSharedData.MapPoints[0];
            OnClickPointView(levelMapGenerator._viewsByPosition[point.Position]);
            Save();
        }

        private void SubscribeToViews()
        {
            foreach (var (_, value) in levelMapGenerator._viewsByPosition)
            {
                value.OnClick += OnClickPointView;
            }
        }

        private void OnClickPointView(MapPointView pointView)
        {
            if (_selectedView)
            {
                _selectedView.SetSelected(false);
            }

            _selectedView = pointView;
            _selectedView.SetSelected(true);

            var idx = _sceneSharedData.MapPoints.IndexOf(pointView.MapPoint);
            _persistentPlayerData.CurMapPointIndex = idx;
        }
    }
}