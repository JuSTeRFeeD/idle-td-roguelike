using Project.Runtime.Core;
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
            WebSaveSystem.PlayerProgressData.mapSave = _persistentPlayerData.MapData;
            WebSaveSystem.PlayerProgressData.curMapPointIndex = _persistentPlayerData.CurMapPointIndex;
            
            WebSaveSystem.PlayerProgressData.hardBalance = _persistentPlayerData.HardCurrency.Balance;
            WebSaveSystem.PlayerProgressData.softBalance = _persistentPlayerData.SoftCurrency.Balance;
            
            WebSaveSystem.PlayerProgressData.commonChestCount = _persistentPlayerData.Chests.CommonChestCount;
            WebSaveSystem.PlayerProgressData.epicChestCount = _persistentPlayerData.Chests.EpicChestCount;
            
            WebSaveSystem.SaveProfile();
            
            // TODO: save on server using ICoroutineRunner
        }

        public void Load()
        {
            // TODO: load from server
            
            WebSaveSystem.Initialize();
            var data = WebSaveSystem.PlayerProgressData;
            
            _persistentPlayerData.MapData = data.mapSave;
            _persistentPlayerData.CurMapPointIndex = data.curMapPointIndex;
            
            _persistentPlayerData.HardCurrency.Add(data.hardBalance);
            _persistentPlayerData.SoftCurrency.Add(data.softBalance);

            _persistentPlayerData.Chests.AddChest(ChestType.Common, data.commonChestCount);
            _persistentPlayerData.Chests.AddChest(ChestType.Epic, data.epicChestCount);
        }
    }
}