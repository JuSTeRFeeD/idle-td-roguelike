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
            while (result.Count < 3)
            {
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