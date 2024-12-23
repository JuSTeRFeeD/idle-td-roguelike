using Project.Runtime.Scriptable.Shop;
using UnityEngine;

namespace Prefabs.Lobby.Skills_Meta
{
    [System.Serializable]
    public class SkillNode
    {
        public SkillType skillType;
        public float value;
        public CurrencyTuple price = new();
        [Space]
        public Vector2Int position;
    }

    public enum SkillType
    {
        None = 0,
        Root = 1,
        AddUnitsCount = 2,
        CriticalChance = 3,
        CriticalDamage = 4,
        AllTowersDamage = 5,
        AllTowersHealth = 6,
        // todo unit gathering double drop chance
    } 
}