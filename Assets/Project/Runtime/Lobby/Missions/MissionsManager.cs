using System;
using Project.Runtime.Lobby.Missions.MissionsWithTimer;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using VContainer;

namespace Project.Runtime.Lobby.Missions
{
    public class MissionsManager
    {
        private readonly ServerTime _serverTime;
        private readonly MissionTimer _missionTimer;
        private readonly PersistentPlayerData _persistentPlayerData;
        private readonly MissionsDatabase _missionsDatabase;
        private readonly ISaveManager _saveManager;
        
        public TimedMissionsManager DailyMissionsManager { get; private set; }
        public TimedMissionsManager WeeklyMissionsManager { get; private set; }

        public event Action OnRefreshed;
        
        [Inject]
        public MissionsManager(ServerTime serverTime, MissionTimer missionTimer, 
            PersistentPlayerData persistentPlayerData, MissionsDatabase missionsDatabase, ISaveManager saveManager)
        {
            _serverTime = serverTime;
            _missionTimer = missionTimer;
            _persistentPlayerData = persistentPlayerData;
            _missionsDatabase = missionsDatabase;
            _saveManager = saveManager;
        }

        public void Initialize()
        {
            DailyMissionsManager = new TimedMissionsManager(_serverTime,  _missionTimer, 
                TimedMissionsType.Daily, _persistentPlayerData, _missionsDatabase);
            WeeklyMissionsManager = new TimedMissionsManager(_serverTime, _missionTimer, 
                TimedMissionsType.Weekly, _persistentPlayerData, _missionsDatabase);
            _saveManager.Save();
        }

        public void Refresh()
        {
            DailyMissionsManager.Refresh();
            WeeklyMissionsManager.Refresh();
            _saveManager.Save();
            OnRefreshed?.Invoke();
        }

        public void Collect(TimedMissionsType timedMissionsType, string missionId)
        {
            var manager = timedMissionsType switch
            {
                TimedMissionsType.Daily => DailyMissionsManager,
                TimedMissionsType.Weekly => WeeklyMissionsManager,
                _ => throw new ArgumentOutOfRangeException(nameof(timedMissionsType), timedMissionsType, null)
            };
            if (!manager.Collect(missionId)) return;
            Refresh();
            OnRefreshed?.Invoke();
        }
    }
}