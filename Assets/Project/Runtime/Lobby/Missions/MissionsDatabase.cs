using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Missions;
using UnityEngine;

namespace Project.Runtime.Lobby.Missions
{
    public class MissionsDatabase : GenericDatabase<MissionConfig>
    {
        public MissionsDatabase()
        {
            var items = Resources.LoadAll<MissionConfig>("Missions/Daily");
            foreach (var buildingConfig in items)
            {
                ItemsById.Add(buildingConfig.uniqueID, buildingConfig);
            }
            Debug.Log($"[MissionsDatabase] Configs: {items.Length}");
        }
    }
}