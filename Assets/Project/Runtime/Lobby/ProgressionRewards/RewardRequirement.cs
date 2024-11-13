using System;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Shop;
using Sirenix.OdinInspector;

namespace Project.Runtime.Lobby.ProgressionRewards
{
    [Serializable]
    public class RewardRequirement
    {
        public bool isPlayerStatisticRequirement;

        public bool IsCurrencyRequirement => !isPlayerStatisticRequirement;
        
        [ShowIf("isPlayerStatisticRequirement")]
        public GlobalStatisticsType globalStatisticsType;
        [ShowIf("isPlayerStatisticRequirement")]
        public long target;

        [ShowIf("IsCurrencyRequirement")] 
        public CurrencyTuple currencyTuple;
    }
}