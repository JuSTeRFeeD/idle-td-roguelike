using System;
using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Scriptable.Shop;

namespace Project.Runtime.Services.PlayerProgress
{
    public class PersistentPlayerData
    {
        public string MapData; // LevelMapGenerator.cs Serialization
        public int CurMapPointIndex;
        public int CompletedMapsCount;
        
        public readonly Dictionary<CurrencyConfig, Wallet> WalletByCurrency = new();
        
        public List<CardSaveData> InventoryCards = new();

        public Chests Chests = new();

        public PersistentPlayerData(IEnumerable<CurrencyConfig> gameCurrencies)
        {
            foreach (var currencyConfig in gameCurrencies)
            {
                WalletByCurrency.Add(currencyConfig, new Wallet(currencyConfig));
            }
        }

        public Wallet GetWalletByCurrencyId(string currencyId)
        {
            return WalletByCurrency.FirstOrDefault(i => i.Key.uniqueID == currencyId).Value;
        }
        
        public CardSaveData GetCardSaveDataByCardId(string id)
        {
            return InventoryCards.FirstOrDefault(i => i.id == id);
        }

        public void AddCardAmountToInventory(CardConfig cardConfig, int amount = 1)
        {
            var data = GetCardSaveDataByCardId(cardConfig.uniqueID);
            data.amount += amount;
        }
    }

    public class Chests
    {
        public int CommonChestCount { get; private set; }
        public int EpicChestCount { get; private set; }

        public event Action OnChange;

        public void AddChest(ChestType chestType, int amount = 1)
        {
            switch (chestType)
            {
                case ChestType.Common:
                    CommonChestCount += amount;
                    break;
                case ChestType.Epic:
                    EpicChestCount += amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(chestType), chestType, null);
            }
            OnChange?.Invoke();
        }

        public bool TakeChest(ChestType chestType, int amount = 1)
        {
            switch (chestType)
            {
                case ChestType.Common:
                    if (CommonChestCount < amount) return false;
                    CommonChestCount -= amount;
                    break;
                case ChestType.Epic:
                    if (EpicChestCount < amount) return false;
                    EpicChestCount -= amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(chestType), chestType, null);
            }

            OnChange?.Invoke();
            return true;
        }
    }

    [Serializable]
    public class CardSaveData
    {
        public string id;
        
        public bool isOpen;
        public int equippedAtSlot = -1;
        
        public int level;
        public int amount; // amount for upgrades
    }
}