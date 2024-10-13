using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.TimeManagement;
using Project.Runtime.Lobby.Map;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Scellecs.Morpeh;
using UnityEngine;
using VContainer;

namespace Project.Runtime.ECS.Systems.GameCycle
{
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
    [Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
    public class GameFinishedSystem : ISystem
    {
        [Inject] private GameFinishedPanel _gameFinishedPanel;
        [Inject] private LevelFinishedDrops _levelFinishedDrops;
        [Inject] private CameraController _cameraController;
        [Inject] private SceneSharedData _sceneSharedData;
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;

        private Filter _gameFinishedFilter;
        private Filter _statisticsFilter;
        private bool _done;
        public World World { get; set; }

        public void OnAwake()
        {
            _gameFinishedFilter = World.Filter
                .With<GameFinishedTag>()
                .Build();
            
            _statisticsFilter = World.Filter
                .With<StatisticTag>()
                .With<TotalPlacedTowersStatistic>()
                .With<TotalDealtDamageStatistic>()
                .With<TotalKilledEnemiesStatistic>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _gameFinishedFilter)
            {
                var isWin = entity.Has<GameWinTag>();

                if (isWin)
                {
                    // Save point as completed
                    _sceneSharedData.MapPoints[_persistentPlayerData.CurMapPointIndex].IsCompleted = true;
                    _persistentPlayerData.MapData = LevelMapGenerator.Serialize(_sceneSharedData.MapPoints);
                }
                
                // Drops
                GenerateAndGiveDrops(isWin, out var randomCardDrop, out var currencyDrops);
                
                // Stats
                var statisticsFilter = _statisticsFilter.First();
                ref readonly var placedTowers = ref statisticsFilter.GetComponent<TotalPlacedTowersStatistic>().Value; 
                ref readonly var dealtDamage = ref statisticsFilter.GetComponent<TotalDealtDamageStatistic>().Value; 
                ref readonly var killedEnemies = ref statisticsFilter.GetComponent<TotalKilledEnemiesStatistic>().Value; 
                
                Debug.Log("Stats");
                Debug.Log($"placedTowers {placedTowers}");
                Debug.Log($"dealtDamage {dealtDamage}");
                Debug.Log($"killedEnemies {killedEnemies}");
                
                if (isWin) _persistentPlayerData.PlayerStatistics.AddStatistics(GlobalStatisticsType.CompletedLevels);
                _persistentPlayerData.PlayerStatistics.AddStatistics(GlobalStatisticsType.PlacedTowers, placedTowers);
                _persistentPlayerData.PlayerStatistics.AddStatistics(GlobalStatisticsType.KilledUnits, killedEnemies);
                _persistentPlayerData.PlayerStatistics.AddStatistics(GlobalStatisticsType.DealtDamage, dealtDamage);
                
                _saveManager.Save();
                
                // Setup end game panel
                _gameFinishedPanel.SetDrops(currencyDrops, randomCardDrop);
                _gameFinishedPanel.SetStatistics(
                    placedTowers,
                    dealtDamage, 
                    killedEnemies);
                _gameFinishedPanel.SetIsWin(isWin);
                _gameFinishedPanel.Show();

                // Stop world
                _cameraController.SetPosition(new Vector3(31, 0, 31));
                World.UpdateByUnity = false;
                TimeScale.OverrideNormalTimeScale(1f);
                TimeScale.SetTimeScale(1f);
            }
        }

        private void GenerateAndGiveDrops(bool isWin, 
            out CardConfig randomCardDrop,
            out List<CurrencyTuple> currencyDrops)
        {
            DropChancesConfig dropChancesConfig;
            if (isWin)
            {
                dropChancesConfig = _sceneSharedData.MapPoints[_persistentPlayerData.CurMapPointIndex].PointType switch
                {
                    MapPoint.MapPointType.Bonus => _levelFinishedDrops.BonusWinConfig,
                    _ => _levelFinishedDrops.WinConfig
                };
            }
            else dropChancesConfig = _levelFinishedDrops.LoseConfig;

            currencyDrops = dropChancesConfig.GetRandomCurrencyDrops();
            foreach (var currencyDrop in currencyDrops)
            {
                _persistentPlayerData.WalletByCurrency[currencyDrop.currencyConfig].Add((ulong)currencyDrop.amount);
            }
            
            if (Random.Range(0, 1f) > 0.6f) // 40% chance to get tower at the end of game
            {
                randomCardDrop = dropChancesConfig.GetRandomCard();
                _persistentPlayerData.AddCardAmountToInventory(randomCardDrop);
            }
            else
            {
                randomCardDrop = null;
            }
        }

        public void Dispose()
        {
        }
    }
}