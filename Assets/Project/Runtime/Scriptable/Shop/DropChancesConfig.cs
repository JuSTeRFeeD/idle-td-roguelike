using System;
using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Scriptable.Currency;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Runtime.Scriptable.Shop
{
    [CreateAssetMenu(menuName = "Game/Loot/DropConfig")]
    public class DropChancesConfig : ScriptableObject
    {
        [Range(0.001f, .6f)]
        [SerializeField] private float legendaryCardChance = 0.5f;
        [Range(0.001f, .6f)]
        [SerializeField] private float epicCardChance = 0.15f;
        [Range(0.001f, .6f)]
        [SerializeField] private float rareCardChance = .4f;
        [Range(0.001f, .6f)]
        [SerializeField] private float uncommonCardChance = .39f;
        [Range(0.001f, .6f)]
        [SerializeField] private float commonCardChance = .55f;
        
        [SerializeField] private ActiveCardsListConfig cardsListConfig;

        [Serializable]
        public class CurrencyDropChance
        {
            [Range(0, 1f)]
            [SerializeField] private float chanceToGiveSomeAmount = 0.5f;
            [SerializeField] private Vector2Int minMaxDropAmount;
            [SerializeField] private CurrencyConfig currency;

            public CurrencyConfig CurrencyConfig => currency;
            public float ChanceToGiveSomeAmount => chanceToGiveSomeAmount;
            public Vector2Int MinMaxDropAmount => minMaxDropAmount;
        }
        [SerializeField] private List<CurrencyDropChance> currencyDrops;
        
        public List<CurrencyDropChance> CurrencyDrops => new(currencyDrops);
        public float LegendaryCardChance => legendaryCardChance;
        public float EpicCardChance => epicCardChance;
        public float RareCardChance => rareCardChance;
        public float UncommonCardChance => uncommonCardChance;
        public float CommonCardChance => commonCardChance;

        public struct CurrencyDrop
        {
            public int Amount;
            public CurrencyConfig CurrencyConfig;
        }
        public List<CurrencyDrop> GetRandomCurrencyDrops()
        {
            List<CurrencyDrop> res = new();
            foreach (var currencyDropChance in currencyDrops)
            {
                if (Random.Range(0, 1f) > currencyDropChance.ChanceToGiveSomeAmount) continue;
                var amount = Random.Range(currencyDropChance.MinMaxDropAmount.x, currencyDropChance.MinMaxDropAmount.y);
                if (amount <= 0) continue;
                res.Add(new CurrencyDrop
                {
                    Amount = amount,
                    CurrencyConfig = currencyDropChance.CurrencyConfig 
                });
            }
            return res;
        }
        
        public CardConfig GetRandomCard(bool nullPossible = false)
        {
            var totalChance = CommonCardChance + UncommonCardChance + RareCardChance + EpicCardChance + LegendaryCardChance;
            
            var randomValue = Random.Range(0f, totalChance);
        
            // Check which rarity the random value falls into
            if (randomValue < CommonCardChance)
            {
                return GetCardByRarity(Rarity.Common, nullPossible);
            }

            if (randomValue < CommonCardChance + UncommonCardChance)
            {
                return GetCardByRarity(Rarity.Uncommon, nullPossible);
            }
            
            if (randomValue < CommonCardChance + UncommonCardChance + RareCardChance)
            {
                return GetCardByRarity(Rarity.Rare, nullPossible);
            }
            
            if (randomValue < CommonCardChance + UncommonCardChance + RareCardChance + EpicCardChance)
            {
                return GetCardByRarity(Rarity.Epic, nullPossible);
            }
            
            return GetCardByRarity(Rarity.Legendary, nullPossible);
        }

        // Method to filter and return a random card from the list by rarity
        private CardConfig GetCardByRarity(Rarity rarity, bool nullPossible)
        {
            var filteredCards = cardsListConfig.Cards.Where(c => c.Rarity == rarity).ToArray();
            
            // не нашли карточки с такой редкостью
            if (filteredCards.Length == 0)
            {
                if (nullPossible) return null;
                switch (rarity)
                {
                    case Rarity.Legendary:
                        return GetCardByRarity(rarity - 1, false);
                    case Rarity.Common:
                        return GetCardByRarity(rarity + 1, false);
                    default:
                        GetCardByRarity(rarity + (Random.Range(0, 2) == 0 ? 1 : -1), false);
                        break;
                }
            }

            return filteredCards[Random.Range(0, filteredCards.Length)];
        }
    }
}