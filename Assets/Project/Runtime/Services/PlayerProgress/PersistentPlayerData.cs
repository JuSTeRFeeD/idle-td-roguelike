using System;
using System.Collections.Generic;
using Project.Runtime.Player;

namespace Project.Runtime.Services.PlayerProgress
{
    public class PersistentPlayerData
    {
        public string MapData; // LevelMapGenerator.cs Serialization
        public int CurMapPointIndex;
        
        public readonly Wallet HardCurrency = new();
        public readonly Wallet SoftCurrency = new();

        public List<CardSaveData> inventoryCards = new();
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