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
            }
        }

        public CardConfig GetRandomCard()
        {
            return _cards.ElementAt(Random.Range(0, _cards.Count)).Key;
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