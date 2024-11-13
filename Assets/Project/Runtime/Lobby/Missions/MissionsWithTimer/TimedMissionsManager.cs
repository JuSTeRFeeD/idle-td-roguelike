using System;
using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Missions;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;

namespace Project.Runtime.Lobby.Missions.MissionsWithTimer
{
    public class TimedMissionsManager
    {
        private readonly PersistentPlayerData _persistentPlayerData;
        private readonly MissionsDatabase _missionsDatabase;
        private readonly TimedMissionsType _timedMissionsType;
        private readonly ServerTime _serverTime;
        private readonly MissionTimer _missionTimer;
        private readonly PlayerStatistics _playerStatistics;
        
        public TimedMissionsManager(ServerTime serverTime, MissionTimer missionTimer, TimedMissionsType timedMissionsType, PersistentPlayerData persistentPlayerData, 
            MissionsDatabase missionsDatabase)
        {
            _serverTime = serverTime;
            _missionTimer = missionTimer;
            _timedMissionsType = timedMissionsType;
            _missionsDatabase = missionsDatabase;
            _persistentPlayerData = persistentPlayerData;
            _playerStatistics = persistentPlayerData.PlayerStatistics;

            if (GetSave() == null)
            {
                GenerateNewMissions();
            }
        }

        public List<MissionConfig> GetMissionsConfigs()
        {
            List<MissionConfig> result = new();
            foreach (var missionId in GetSave().missionIds)
            {
                switch (_timedMissionsType)
                {
                    case TimedMissionsType.Daily:
                        result.Add(_missionsDatabase.GetById(missionId));
                        break;
                    case TimedMissionsType.Weekly:
                        result.Add(_missionsDatabase.GetWeeklyMissionById(missionId));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return result;
        }

        public MissionsSave GetSave()
        {
            return _timedMissionsType switch
            {
                TimedMissionsType.Daily => _persistentPlayerData.DailyMissions,
                TimedMissionsType.Weekly => _persistentPlayerData.WeeklyMissions,
                _ => throw new ArgumentOutOfRangeException()
            }; 
        }

        private void GenerateNewMissions()
        {
            Debug.Log("Generate new missions");
            
            var missionConfigs = _timedMissionsType switch
            {
                TimedMissionsType.Daily => _missionsDatabase.GetAllItems().ToList(),
                TimedMissionsType.Weekly => _missionsDatabase.GetAllWeeklyItems().ToList(),
                _ => throw new ArgumentOutOfRangeException()
            };

            var save = new MissionsSave
            {
                startTime = _serverTime.GetServerTimeLong(),
                completed = new bool[missionConfigs.Count],
                rewarded = new bool[missionConfigs.Count],
                missionIds = new string[missionConfigs.Count],
                valueAtStart = new long[missionConfigs.Count]
            };

            for (var i = 0; i < missionConfigs.Count; i++)
            {
                var config = missionConfigs[i];
                save.completed[i] = false;
                save.rewarded[i] = false;
                save.missionIds[i] = config.uniqueID;

                save.valueAtStart[i] = _playerStatistics.GetStatistic(config.MissionType);

                if (config.MissionType is GlobalStatisticsType.LoginTimes)
                {
                    if (new DateTime(_persistentPlayerData.LastLoginTime).Day != _serverTime.GetServerDateTime().Day)
                    {
                        save.completed[i] = true;
                    }
                }
            }

            save.SortByCompletionAndRewardStatus();
            
            switch (_timedMissionsType)
            {
                case TimedMissionsType.Daily:
                    _persistentPlayerData.DailyMissions = save;
                    break;
                case TimedMissionsType.Weekly:
                    _persistentPlayerData.WeeklyMissions = save;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void Refresh()
        {
            var save = GetSave();
            
            Debug.Log($"is save null {save == null} | mt {_missionTimer == null}");
            
            // Check time to update missions
            var currentServerTime = _serverTime.GetServerTimeLong();

            if (_timedMissionsType is TimedMissionsType.Daily && 
                currentServerTime > _missionTimer.GetNextDailyMissionUpdateUnixTime(save.startTime))
            {
                Debug.Log("Refresh mission");
                GenerateNewMissions();
                return;
            } 
            if (_timedMissionsType is TimedMissionsType.Weekly && 
                  currentServerTime > _missionTimer.GetNextWeeklyMissionUpdateUnixTime(save.startTime))
            {
                Debug.Log("Refresh mission");
                GenerateNewMissions();
                return;
            }
            
            Debug.Log($"len {save.missionIds.Length}");
            
            // Mark Completed and Sort
            for (var i = 0; i < save.valueAtStart.Length; i++)
            {
                if (save.completed[i] || save.rewarded[i]) continue;
                var missionConfig = _timedMissionsType switch
                {
                    TimedMissionsType.Daily => _missionsDatabase.GetById(save.missionIds[i]),
                    TimedMissionsType.Weekly => _missionsDatabase.GetWeeklyMissionById(save.missionIds[i]),
                    _ => throw new ArgumentOutOfRangeException()
                };
                var targetValue = save.valueAtStart[i] + missionConfig.ValueToComplete;
                var currentValue = _persistentPlayerData.PlayerStatistics.GetStatistic(missionConfig.MissionType);
                Debug.Log($"{missionConfig.MissionName} {currentValue} / {targetValue}");
                if (currentValue >= targetValue)
                {
                    save.completed[i] = true;
                }
            }
            save.SortByCompletionAndRewardStatus();
        }

        public bool Collect(string id)
        {
            var save = GetSave();
            for (var i = 0; i < save.missionIds.Length; i++)
            {
                if (!save.missionIds[i].Equals(id)) continue;
                if (!save.completed[i]) continue;
                
                save.rewarded[i] = true;
                var missionConfig = _timedMissionsType switch
                {
                    TimedMissionsType.Daily => _missionsDatabase.GetById(save.missionIds[i]),
                    TimedMissionsType.Weekly => _missionsDatabase.GetWeeklyMissionById(save.missionIds[i]),
                    _ => throw new ArgumentOutOfRangeException()
                };
                var wallet = _persistentPlayerData.GetWalletByCurrencyId(missionConfig.RewardCurrency.currencyConfig.uniqueID);
                wallet.Add((ulong)missionConfig.RewardCurrency.amount);

                _persistentPlayerData.PlayerStatistics.AddStatistics(GlobalStatisticsType.CompletedMissions);
                
                return true;
            }

            return false;
        }
    }
}