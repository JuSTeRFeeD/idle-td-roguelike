using System;
using System.Collections.Generic;
using Project.Runtime.Core.Data;
using Project.Runtime.Lobby.Missions.MissionsWithTimer;
using Project.Runtime.Player;
using Project.Runtime.Services.PlayerProgress;

namespace Project.Runtime.Services.Saves
{
    [Serializable]
    public class PlayerProgressData
    {
        public bool isInGameTutorialCompleted;
        
        public string mapSave;
        public int curMapPointIndex;
        public int completedMapsCount;

        public bool autoUpgradeTowersChecked;
        
        public DictionarySerializeContainer<string, ulong> balanceByCurrencyId = new();
        
        public List<CardSaveData> inventoryCards = new();

        public DictionarySerializeContainer<GlobalStatisticsType, long> globalStatistics;
        public MissionsSave dailyMissions;
        public MissionsSave weeklyMissions;
        
        public bool[] dailyRewardProgressCollected;
        public bool[] weeklyRewardProgressCollected;
    }
}