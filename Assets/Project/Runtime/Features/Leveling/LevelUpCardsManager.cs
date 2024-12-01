using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Card;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Features.Leveling
{
    public class LevelUpCardsManager
    {
        private readonly Dictionary<CardConfig, int> _cards = new();

        private bool _isFirstCardsRolled = false;
        
        // не оч хорошо что он открытый для записи наверн, но пока пусть так
        public readonly Dictionary<string, int> AppliesCountByPerkUniqueId = new();

        private int _takenTowerCards = 0;
        private bool _towerGivenAtThisLevel;
        private const int GiveTowersTillTakenCount = 3;
        
        [Inject]
        public LevelUpCardsManager(PlayerDeck playerDeck)
        {
            foreach (var card in playerDeck.GetCardsForGame())
            {
                _cards.Add(card, card.MaxPerGame);
                if (card.SubCardConfigs != null)
                {
                    foreach (var subCard in card.SubCardConfigs)
                    {
                        _cards.Add(subCard, subCard.MaxPerGame);
                    }
                }
            }
        }

        public List<CardConfig> GetRandomCard()
        {
            List<CardConfig> result = new();
            
            if (!_isFirstCardsRolled)
            {
                _isFirstCardsRolled = true;
                var buildings = _cards.Where(i => i.Key.IsBuilding).ToList();
                for (var i = 0; i < 3; i++)
                {
                    result.Add(buildings.ElementAt(Random.Range(0, buildings.Count)).Key);
                }
                return result;
            }

            _towerGivenAtThisLevel = false;
            var count = _cards.Count > 2 ? 3 : _cards.Count;
            while (result.Count < count)
            {
                // Даем как минимум одну карточку тавера (пока х3 раза)
                if (!_towerGivenAtThisLevel && _takenTowerCards < GiveTowersTillTakenCount)
                {
                    var buildings = _cards.Where(i => i.Key.IsBuilding).ToList();
                    result.Add(buildings.ElementAt(Random.Range(0, buildings.Count)).Key);
                    _takenTowerCards++;
                    continue;
                }
                
                // Случайная карточка
                var rndCard = _cards.ElementAt(Random.Range(0, _cards.Count)).Key;
                while (result.Contains(rndCard))
                {
                    rndCard = _cards.ElementAt(Random.Range(0, _cards.Count)).Key;
                }
                result.Add(rndCard);
            }
            return result;
        }

        public void DecreaseDropCount(CardConfig cardConfig)
        {
            _cards[cardConfig]--;
            if (_cards[cardConfig] <= 0)
            {
                _cards.Remove(cardConfig);
            }
        }
    }
}