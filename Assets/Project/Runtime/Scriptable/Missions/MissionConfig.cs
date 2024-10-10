using Project.Runtime.Scriptable.Shop;
using UnityEngine;

namespace Project.Runtime.Scriptable.Missions
{
    [CreateAssetMenu(menuName = "Meta/Mission")]
    public class MissionConfig : UniqueConfig
    {
        [SerializeField] private MissionType missionType;
        [SerializeField] private string missionName;
        [SerializeField] private int valueToComplete;
        [Space] 
        [SerializeField] private CurrencyTuple reward;

        public MissionType MissionType => missionType;
        public string MissionName => missionName;
        public int ValueToComplete => valueToComplete;
        public CurrencyTuple RewardCurrency => reward;
    }
}