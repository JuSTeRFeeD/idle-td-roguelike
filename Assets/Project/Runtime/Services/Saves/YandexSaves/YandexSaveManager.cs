using System.Collections.Generic;
using Project.Runtime.Core;
using Project.Runtime.Core.Data;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves.YandexSaves.FileSavingSystem;
using VContainer;

namespace Project.Runtime.Services.Saves.YandexSaves
{
    public class YandexSaveManager : ISaveManager
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ICoroutineRunner _coroutineRunner;
        
        public void Save()
        {
            // Map
            WebSaveSystem.PlayerProgressData.mapSave = _persistentPlayerData.MapData;
            WebSaveSystem.PlayerProgressData.curMapPointIndex = _persistentPlayerData.CurMapPointIndex;
            WebSaveSystem.PlayerProgressData.completedMapsCount = _persistentPlayerData.CompletedMapsCount;
            
            // Balance
            WebSaveSystem.PlayerProgressData.balanceByCurrencyId?.Clear();
            var balanceDict = new Dictionary<string, int> ();
            foreach (var (key, value) in _persistentPlayerData.WalletByCurrency)
            {
                balanceDict.Add(key.uniqueID, value.Balance);
            }
            WebSaveSystem.PlayerProgressData.balanceByCurrencyId =
                new DictionarySerializeContainer<string, int>(balanceDict);
            
            // Chests
            WebSaveSystem.PlayerProgressData.commonChestCount = _persistentPlayerData.Chests.CommonChestCount;
            WebSaveSystem.PlayerProgressData.epicChestCount = _persistentPlayerData.Chests.EpicChestCount;
            
            // Inventory
            WebSaveSystem.PlayerProgressData.inventoryCards = _persistentPlayerData.InventoryCards;
            
            WebSaveSystem.SaveProfile();
            
            // TODO: save on server using ICoroutineRunner
        }

        public void Load()
        {
            // TODO: load from server
            
            WebSaveSystem.Initialize();
            var data = WebSaveSystem.PlayerProgressData;
            
            // Map
            _persistentPlayerData.MapData = data.mapSave;
            _persistentPlayerData.CurMapPointIndex = data.curMapPointIndex;
            _persistentPlayerData.CompletedMapsCount = data.completedMapsCount;
            
            // Balance
            if (data.balanceByCurrencyId != null)
            {
                foreach (var (key, value) in data.balanceByCurrencyId.ToDictionary())
                {
                    _persistentPlayerData.GetWalletByCurrencyId(key).Add(value);
                }
            }

            // Chest
            _persistentPlayerData.Chests.AddChest(ChestType.Common, data.commonChestCount);
            _persistentPlayerData.Chests.AddChest(ChestType.Epic, data.epicChestCount);

            // Inventory
            _persistentPlayerData.InventoryCards = data.inventoryCards;
        }
    }
}