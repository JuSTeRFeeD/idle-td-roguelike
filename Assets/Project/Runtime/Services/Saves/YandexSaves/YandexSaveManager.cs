using System.Collections.Generic;
using Project.Runtime.Core;
using Project.Runtime.Core.Data;
using Project.Runtime.Player;
using Project.Runtime.Services.PlayerProgress;
using VContainer;
using YG;

namespace Project.Runtime.Services.Saves.YandexSaves
{
    public class YandexSaveManager : ISaveManager
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ICoroutineRunner _coroutineRunner;

        public YandexSaveManager()
        {
            YandexGame.GetDataEvent += Load;
        }
        
        ~YandexSaveManager()
        {
            YandexGame.GetDataEvent -= Load;
        }
        
        public void Save()
        {
            var data = YandexGame.savesData.playerProgressData;
            
            // Map
            data.mapSave = _persistentPlayerData.MapData;
            data.curMapPointIndex = _persistentPlayerData.CurMapPointIndex;
            data.completedMapsCount = _persistentPlayerData.CompletedMapsCount;
            
            // Gameplay
            data.autoUpgradeTowersChecked = _persistentPlayerData.AutoUpgradeTowersChecked;
            
            // Balance
            data.balanceByCurrencyId?.Clear();
            var balanceDict = new Dictionary<string, int> ();
            foreach (var (key, value) in _persistentPlayerData.WalletByCurrency)
            {
                balanceDict.Add(key.uniqueID, value.Balance);
            }
            data.balanceByCurrencyId =
                new DictionarySerializeContainer<string, int>(balanceDict);
            
            // Inventory
            data.inventoryCards = _persistentPlayerData.InventoryCards;
            
            // Stats
            data.globalStatistics = new DictionarySerializeContainer<GlobalStatisticsType, long>(
                _persistentPlayerData.PlayerStatistics._globalStatistics);
            
            YandexGame.SaveProgress();
        }

        public void Load()
        {
            var data = YandexGame.savesData.playerProgressData;
            
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
        }
    }
}