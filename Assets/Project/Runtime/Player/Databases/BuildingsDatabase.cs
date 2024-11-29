using Project.Runtime.Scriptable.Buildings;
using UnityEngine;

namespace Project.Runtime.Player.Databases
{
    public class BuildingsDatabase : GenericDatabase<BuildingConfig>
    {
        public BuildingsDatabase()
        {
            var items = Resources.LoadAll<BuildingConfig>("Configs");
            foreach (var buildingConfig in items)
            {
                ItemsById.Add(buildingConfig.uniqueID, buildingConfig);
            }
        }
    }
}