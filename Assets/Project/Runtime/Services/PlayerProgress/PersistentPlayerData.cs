using System;
using System.Collections.Generic;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Shop;

namespace Project.Runtime.Services.PlayerProgress
{
    public class PersistentPlayerData
    {
        public string MapData; // LevelMapGenerator.cs Serialization
        public int CurMapPointIndex;
        
        public readonly Wallet HardCurrency = new();
        public readonly Wallet SoftCurrency = new();
        
        public List<CardSaveData> inventoryCards = new();

        public Chests Chests = new();
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