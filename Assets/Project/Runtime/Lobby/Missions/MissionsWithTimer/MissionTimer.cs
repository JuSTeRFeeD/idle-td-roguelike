using System;
using VContainer;

namespace Project.Runtime.Lobby.Missions.MissionsWithTimer
{
   public class MissionTimer
    {
        private readonly ServerTime _serverTime;

        [Inject]
        public MissionTimer(ServerTime serverTime)
        {
            _serverTime = serverTime;
        }

        // Получить Unix timestamp для следующего обновления ежедневных миссий
        public long GetNextDailyMissionUpdateUnixTime(long missionStartTime)
        {
            DateTime currentServerTime = _serverTime.GetServerDateTime();
            DateTime nextDailyUpdate = GetNextDailyUpdateTime(currentServerTime);
            return DateTimeToUnixTimeStamp(nextDailyUpdate);
        }

        // Получить Unix timestamp для следующего обновления еженедельных миссий
        public long GetNextWeeklyMissionUpdateUnixTime(long missionStartTime)
        {
            DateTime currentServerTime = _serverTime.GetServerDateTime();
            DateTime nextWeeklyUpdate = GetNextWeeklyUpdateTime(currentServerTime);
            return DateTimeToUnixTimeStamp(nextWeeklyUpdate);
        }
        
        // Преобразование DateTime в Unix timestamp (long)
        private long DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime - epochStart).TotalMilliseconds;
        }
       
        // Вычисление следующего обновления ежедневных миссий (в 6 утра следующего дня)
        private DateTime GetNextDailyUpdateTime(DateTime currentTime)
        {
            DateTime nextUpdate = currentTime.Date.AddHours(6); // 6 утра текущего дня
            if (currentTime >= nextUpdate)
            {
                // Если текущее время уже после 6 утра, обновление будет на следующий день в 6 утра
                nextUpdate = nextUpdate.AddDays(1);
            }
            return nextUpdate;
        }

        // Вычисление следующего обновления еженедельных миссий (в 6 утра ближайшего воскресенья)
        private DateTime GetNextWeeklyUpdateTime(DateTime currentTime)
        {
            // Находим ближайшее воскресенье
            int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)currentTime.DayOfWeek + 7) % 7;
            DateTime nextUpdate = currentTime.Date.AddDays(daysUntilSunday).AddHours(6); // Ближайшее воскресенье в 6 утра

            if (currentTime >= nextUpdate)
            {
                // Если текущее время уже после 6 утра в воскресенье, сдвигаем на следующее воскресенье
                nextUpdate = nextUpdate.AddDays(7);
            }
            return nextUpdate;
        }
    }
}