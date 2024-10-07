using System;
using System.Collections.Generic;
using Project.Runtime.Core.Data;
using Project.Runtime.Services.PlayerProgress;

namespace Project.Runtime.Services.Saves
{
    [Serializable]
    public class PlayerProgressData
    {
        public string mapSave;
        public int curMapPointIndex;
        public int completedMapsCount;

        public bool autoUpgradeTowersChecked;
        
        public DictionarySerializeContainer<string, int> balanceByCurrencyId;
        
        public List<CardSaveData> inventoryCards = new();

        public int commonChestCount;
        public int epicChestCount;
    }
}