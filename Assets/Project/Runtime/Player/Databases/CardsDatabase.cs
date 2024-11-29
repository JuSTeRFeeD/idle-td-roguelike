using Project.Runtime.Scriptable.Card;
using UnityEngine;

namespace Project.Runtime.Player.Databases
{
    public class CardsDatabase : GenericDatabase<CardConfig>
    {
        public CardsDatabase()
        {
            var items = Resources.LoadAll<CardConfig>("Configs");
            foreach (var buildingConfig in items)
            {
                ItemsById.Add(buildingConfig.uniqueID, buildingConfig);
            }
        }
    }
}