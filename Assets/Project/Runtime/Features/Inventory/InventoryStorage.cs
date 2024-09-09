using System;
using System.Collections.Generic;
using Project.Runtime.Scriptable.Card;

namespace Project.Runtime.Features.Inventory
{
    public class InventoryStorage
    {
        private readonly List<CardConfig> _cards = new();

        public event Action OnCardsChange; 
        
        public List<CardConfig> GetBuildingsList() => new(_cards);
        
        public void AddCard(CardConfig cardConfig)
        {
            _cards.Add(cardConfig);
            OnCardsChange?.Invoke();
        }

        public void RemoveCard(string cardConfigId)
        {
            var idx = _cards.FindIndex(i => i.uniqueID == cardConfigId);
            if (idx == -1) return;
            _cards.RemoveAt(idx);
            OnCardsChange?.Invoke();
        }

    }
}