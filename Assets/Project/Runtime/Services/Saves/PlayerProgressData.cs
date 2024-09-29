using System;
using System.Collections.Generic;
using Project.Runtime.Services.PlayerProgress;

namespace Project.Runtime.Services.Saves
{
    [Serializable]
    public class PlayerProgressData
    {
        public string mapSave;
        public int curMapPointIndex;
        
        public int hardBalance;
        public int softBalance;
        
        public List<CardSaveData> inventoryCards = new();
    }
}