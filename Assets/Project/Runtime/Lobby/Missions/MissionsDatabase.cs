using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Missions;
using UnityEngine;

namespace Project.Runtime.Lobby.Missions
{
    public class MissionsDatabase : GenericDatabase<MissionConfig>
    {
        private readonly Dictionary<string, MissionConfig> _weeklyItemsById = new();
        
        public MissionsDatabase()
        {
            var items = Resources.LoadAll<MissionConfig>("Missions/Daily");
            foreach (var missionConfig in items)
            {
                ItemsById.Add(missionConfig.uniqueID, missionConfig);
            }
            
            var weeklyItems = Resources.LoadAll<MissionConfig>("Missions/Weekly");
            foreach (var missionConfig in weeklyItems)
            {
                _weeklyItemsById.Add(missionConfig.uniqueID, missionConfig);
            }
        }

        public IEnumerable<MissionConfig> GetAllWeeklyItems()
        {
            return _weeklyItemsById.Select(i => i.Value);
        }
        
        public MissionConfig GetWeeklyMissionById(string id)
        {
            return _weeklyItemsById.GetValueOrDefault(id);
        }
    }
}