using System.Collections;
using System.Collections.Generic;
using Project.Runtime.Core;
using Project.Runtime.Core.Data;
using Project.Runtime.Player;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;
using VContainer;
using YG;

namespace Project.Runtime.Services.Saves.YandexSaves
{
    public class YandexSaveManager : ISaveManager
    {
        private readonly PersistentPlayerData _persistentPlayerData;
        private readonly ICoroutineRunner _coroutineRunner;

        private Coroutine _saveRoutine;
        
        [Inject]
        public YandexSaveManager(PersistentPlayerData persistentPlayerData, ICoroutineRunner coroutineRunner)
        {
            _persistentPlayerData = persistentPlayerData;
            _coroutineRunner = coroutineRunner;
            YG2.onGetSDKData += Load;
        }
        
        ~YandexSaveManager()
        {
            YG2.onGetSDKData -= Load;
        }
        
        public void Save(bool force = false)
        {
            var data = YG2.saves.playerProgressData;
            
            data.isInGameTutorialCompleted = _persistentPlayerData.IsInGameTutorialCompleted;
            
            // Map
            data.mapSave = _persistentPlayerData.MapData;
            data.curMapPointIndex = _persistentPlayerData.CurMapPointIndex;
            data.completedMapsCount = _persistentPlayerData.CompletedMapsCount;
            
            // Gameplay
            data.autoUpgradeTowersChecked = _persistentPlayerData.AutoUpgradeTowersChecked;
            
            // Balance
            data.balanceByCurrencyId?.Clear();
            var balanceDict = new Dictionary<string, ulong>();
            foreach (var (key, value) in _persistentPlayerData.WalletByCurrency)
            {
                balanceDict.Add(key.uniqueID, value.Balance);
            }
            data.balanceByCurrencyId = new DictionarySerializeContainer<string, ulong>(balanceDict);
            
            // Inventory
            data.inventoryCards = _persistentPlayerData.InventoryCards;
            
            // Stats
            data.globalStatistics = new DictionarySerializeContainer<GlobalStatisticsType, long>(
                _persistentPlayerData.PlayerStatistics._globalStatistics);
            
            // Missions
            data.dailyMissions = _persistentPlayerData.DailyMissions;
            data.weeklyMissions = _persistentPlayerData.WeeklyMissions;
            data.dailyRewardProgressCollected = _persistentPlayerData.DailyRewardProgressCollected;
            data.weeklyRewardProgressCollected = _persistentPlayerData.WeeklyRewardProgressCollected;

            if (force)
            {
                YG2.SaveProgress();
            }
            else
            {
                if (_saveRoutine != null) _coroutineRunner.StopCoroutine(_saveRoutine);
                _coroutineRunner.StartCoroutine(DelayedSave());
            }
        }

        private IEnumerator DelayedSave()
        {
            yield return new WaitForSeconds(4.5f);
            YG2.SaveProgress();
        }

        public void Load()
        {
            Debug.Log("LOAD");

            YG2.saves.playerProgressData ??= new PlayerProgressData();
            var data = YG2.saves.playerProgressData;

            _persistentPlayerData.IsInGameTutorialCompleted = data.isInGameTutorialCompleted; 
            
            // Map
            _persistentPlayerData.MapData = data.mapSave;
            _persistentPlayerData.CurMapPointIndex = data.curMapPointIndex;
            _persistentPlayerData.CompletedMapsCount = data.completedMapsCount;
            
            // Gameplay
            _persistentPlayerData.AutoUpgradeTowersChecked = data.autoUpgradeTowersChecked;
            
            // Balance
            if (data.balanceByCurrencyId != null)
            {
                foreach (var (key, value) in data.balanceByCurrencyId.ToDictionary())
                {
                    _persistentPlayerData.GetWalletByCurrencyId(key).Add(value);
                }
            }

            // Inventory
            _persistentPlayerData.InventoryCards = data.inventoryCards;
            
            // Stats
            if (data.globalStatistics != null)
            {
                _persistentPlayerData.PlayerStatistics.Initialize(data.globalStatistics.ToDictionary());
            }

            // Missions
            _persistentPlayerData.DailyMissions = data.dailyMissions;
            _persistentPlayerData.WeeklyMissions = data.weeklyMissions;
            _persistentPlayerData.DailyRewardProgressCollected = data.dailyRewardProgressCollected;
            _persistentPlayerData.WeeklyRewardProgressCollected = data.weeklyRewardProgressCollected;
        }
    }
}