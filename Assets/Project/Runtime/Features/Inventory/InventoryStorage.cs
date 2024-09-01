using System;
using System.Collections.Generic;
using Project.Runtime.Scriptable.Card;

namespace Project.Runtime.Features.Inventory
{
    public class InventoryStorage
    {
        private readonly List<CardConfig> _cards = new();

        public event Action OnCardsChange; 
        
        public void AddCard(CardConfig cardConfig)
        {
            _cards.Add(cardConfig);
            OnCardsChange?.Invoke();
        }

        public List<CardConfig> GetBuildingsList() => new(_cards);
    }
}