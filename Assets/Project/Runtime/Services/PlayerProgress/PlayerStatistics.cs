using System.Collections.Generic;
using Project.Runtime.Player;

namespace Project.Runtime.Services.PlayerProgress
{
    public class PlayerStatistics
    {
        public Dictionary<GlobalStatisticsType, long> _globalStatistics = new();

        public void Initialize(Dictionary<GlobalStatisticsType, long> statistics)
        {
            _globalStatistics = statistics;
        }
        
        public long GetStatistic(GlobalStatisticsType type)
        {
            return _globalStatistics.GetValueOrDefault(type, 0);
        }
        
        public void AddStatistics(GlobalStatisticsType type, long amount = 1)
        {
            if (!_globalStatistics.TryAdd(type, amount))
            {
                _globalStatistics[type] += amount;
            }
        }
    }
}