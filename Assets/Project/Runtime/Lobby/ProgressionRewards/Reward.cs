using System;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.Shop;
using Sirenix.OdinInspector;

namespace Project.Runtime.Lobby.ProgressionRewards
{
    [Serializable]
    public struct Reward
    {
        [Title("Requirement")]
        public RewardRequirement requirement;
        
        [Title("Reward")]
        public bool isCurrencyReward;
        private bool IsCardReward => !isCurrencyReward;
        
        [ShowIf("isCurrencyReward")]
        public CurrencyTuple currencyTuple;
        [ShowIf("IsCardReward")]
        public CardConfig cardConfig;
    }
}