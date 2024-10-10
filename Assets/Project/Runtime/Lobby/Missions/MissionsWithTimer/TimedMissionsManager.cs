using System;
using System.Collections.Generic;
using Project.Runtime.Scriptable.Missions;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;
using VContainer;
using YG;

namespace Project.Runtime.Lobby.Missions.MissionsWithTimer
{
    public class TimedMissionsManager : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;

        [SerializeField] private TimedMissionsType timedMissionsType;
        [SerializeField] private List<MissionConfig> missionsConfigs;

        private long _serverTime;
        private float _clientStartTime;
        
        private void Start()
        {
            _serverTime = YandexGame.ServerTime();
            _clientStartTime = Time.time;

            if (_persistentPlayerData.DailyMissions == null)
            {
                GenerateNewMissions();
            }
            else
            {
                LoadMissions();
            }
        }

        private void Update()
        {
            // Вычисляем текущее "виртуальное" серверное время
            var currentTime = _serverTime + (long)((Time.time - _clientStartTime) * 1000);
            var dateTime = UnixTimeStampToDateTime(currentTime);
            // timeText.text = "Время сервера: " + dateTime.ToString("HH:mm:ss");
        }
        
        // Метод для преобразования Unix timestamp в DateTime
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epochStart.AddMilliseconds(unixTimeStamp);
        }

        private void LoadMissions()
        {
        }

        private void GenerateNewMissions()
        {
            
        }
    }

    [Serializable]
    public class MissionsSave
    {
        public long startTime;
        public string[] missionIds;
        public int[] valueAtStart;
        public bool[] completed;
    }
}