using System;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;
using VContainer;
using YG;

namespace Project.Runtime.Lobby.Missions
{
    public class ServerTime
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        
        private long _serverTime;
        private float _clientStartTime;

        public void Refresh()
        {
            _serverTime = YandexGame.ServerTime();
            _clientStartTime = Time.time;
            _persistentPlayerData.LastLoginTime = _serverTime;
        }

        public long GetServerTimeLong()
        {
            return _serverTime + (long)((Time.time - _clientStartTime) * 1000);
        }
        
        public DateTime GetServerDateTime()
        {
            var currentTime = _serverTime + (long)((Time.time - _clientStartTime) * 1000);
            return UnixTimeStampToDateTime(currentTime);
        }

        public string GetServerServerTimeFormat()
        {
            var currentTime = _serverTime + (long)((Time.time - _clientStartTime) * 1000);
            var dateTime = UnixTimeStampToDateTime(currentTime);
            return dateTime.ToString("HH:mm:ss");
        }

        /// Метод для преобразования Unix timestamp в DateTime
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return  DateTime.UnixEpoch.AddMilliseconds(unixTimeStamp);
        }
    }
}