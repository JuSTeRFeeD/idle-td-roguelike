using System;
using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Lobby.Missions.MissionsWithTimer;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.Currency;

namespace Project.Runtime.Services.PlayerProgress
{
    public class PersistentPlayerData
    {
        public bool IsInGameTutorialCompleted;
        
        public string MapData; // LevelMapGenerator.cs Serialization
        public int CurMapPointIndex;
        public int CompletedMapsCount;

        public bool AutoUpgradeTowersChecked;
        
        public readonly Dictionary<CurrencyConfig, Wallet> WalletByCurrency = new();
        public event Action<Wallet> OnChangeWalletBalance;
        
        public List<CardSaveData> InventoryCards = new();

        public readonly PlayerStatistics PlayerStatistics = new();
        public MissionsSave DailyMissions;
        public MissionsSave WeeklyMissions;
        public long LastLoginTime;
        public bool[] DailyRewardProgressCollected;
        public bool[] WeeklyRewardProgressCollected;

        public PlayerSkills PlayerSkills = new();

        public PersistentPlayerData(CurrencyConfig[] gameCurrencies)
        {
            foreach (var currencyConfig in gameCurrencies)
            {
                var wallet = new Wallet(currencyConfig);
                WalletByCurrency.Add(currencyConfig, wallet);
                wallet.OnChange += (_, _) => OnChangeWalletBalance?.Invoke(wallet);
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
            if (cardConfig == null) return;
            var data = GetCardSaveDataByCardId(cardConfig.uniqueID);
            data.amount += amount;
        }
    }

    [Serializable]
    public class CardSaveData
    {
        public string id;
        
        public bool isOpen;
        public int equippedAtSlot = -1;
        public bool isEquipped;
        
        public int level;
        public int amount; // amount for upgrades
    }
}