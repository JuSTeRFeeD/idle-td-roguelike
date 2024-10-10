using System.Collections.Generic;
using Project.Runtime.ECS.Components;
using Project.Runtime.Features.CameraControl;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.TimeManagement;
using Project.Runtime.Lobby.Map;
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

                _saveManager.Save();
                
                var statisticsFilter = _statisticsFilter.First();
                _gameFinishedPanel.SetDrops(currencyDrops, randomCardDrop);
                _gameFinishedPanel.SetStatistics(
                    statisticsFilter.GetComponent<TotalPlacedTowersStatistic>().Value,
                    statisticsFilter.GetComponent<TotalDealtDamageStatistic>().Value, 
                    statisticsFilter.GetComponent<TotalKilledEnemiesStatistic>().Value);
                _gameFinishedPanel.SetIsWin(isWin);
                _gameFinishedPanel.Show();

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
                _persistentPlayerData.WalletByCurrency[currencyDrop.currencyConfig].Add(currencyDrop.amount);
            }
            
            if (Random.Range(0, 1f) > 0.65f) // 35% chance to get tower at the end of game
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