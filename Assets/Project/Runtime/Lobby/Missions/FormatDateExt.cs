using System;

namespace Project.Runtime.Lobby.Missions
{
    public static class FormatDateExt
    {
        public static string FormatTimeRemaining(this TimeSpan timeRemaining)
        {
            if (timeRemaining.TotalDays >= 1)
            {
                return $"{timeRemaining.Days}д {timeRemaining.Hours}ч";
            }
            if (timeRemaining.TotalHours >= 1)
            {
                return $"{timeRemaining.Hours}ч {timeRemaining.Minutes}м";
            }
            else if (timeRemaining.TotalMinutes >= 1)
            {
                return $"{timeRemaining.Minutes}м {timeRemaining.Seconds}с";
            }
            else
            {
                return $"{timeRemaining.Seconds}с";
            }
        }
    }
}