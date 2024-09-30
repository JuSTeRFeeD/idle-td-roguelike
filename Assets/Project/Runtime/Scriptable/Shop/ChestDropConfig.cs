using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Scriptable.Card;
using UnityEngine;

namespace Project.Runtime.Scriptable.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/ChestDropConfig")]
    public class ChestDropConfig : ScriptableObject
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
        [SerializeField] private float commonCardChance = 55f;
        
        [SerializeField] private List<CardConfig> dropCards;
        
        public List<CardConfig> DropCards => new(dropCards);
        public float LegendaryCardChance => legendaryCardChance;
        public float EpicCardChance => epicCardChance;
        public float RareCardChance => rareCardChance;
        public float UncommonCardChance => uncommonCardChance;
        public float CommonCardChance => commonCardChance;
        
        public CardConfig GetRandomCard()
        {
            var totalChance = CommonCardChance + UncommonCardChance + RareCardChance + EpicCardChance + LegendaryCardChance;
            
            var randomValue = Random.Range(0f, totalChance);
        
            // Check which rarity the random value falls into
            if (randomValue < CommonCardChance)
            {
                return GetCardByRarity(Rarity.Common);
            }

            if (randomValue < CommonCardChance + UncommonCardChance)
            {
                return GetCardByRarity(Rarity.Uncommon);
            }
            
            if (randomValue < CommonCardChance + UncommonCardChance + RareCardChance)
            {
                return GetCardByRarity(Rarity.Rare);
            }
            
            if (randomValue < CommonCardChance + UncommonCardChance + RareCardChance + EpicCardChance)
            {
                return GetCardByRarity(Rarity.Epic);
            }
            
            return GetCardByRarity(Rarity.Legendary);
        }

        // Method to filter and return a random card from the list by rarity
        private CardConfig GetCardByRarity(Rarity rarity)
        {
            var filteredCards = DropCards.Where(c => c.Rarity == rarity).ToList();
            if (filteredCards.Count == 0)
            {
                // Handle empty list case
                return GetRandomCard(); // todo: recursion is possible here
            }

            var randomIndex = Random.Range(0, filteredCards.Count);
            return filteredCards[randomIndex];
        }
    }
}