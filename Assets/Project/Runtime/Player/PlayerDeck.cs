using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Services.PlayerProgress;
using VContainer;

namespace Project.Runtime.Player
{
    public class DeckCard
    {
        public CardConfig CardConfig;
        public CardSaveData CardSaveData;
    }
    
    public class PlayerDeck
    {
        private readonly PersistentPlayerData _persistentPlayerData;
        private readonly ActiveCardsListConfig _commonCards;
        private readonly ActiveCardsListConfig _firstTimeCardsListConfig;

        private readonly List<DeckCard> _inventoryCards = new();

        public PlayerDeck(PersistentPlayerData playerData, ActiveCardsListConfig commonCardsList,
            ActiveCardsListConfig firstTimeCardsList)
        {
            _persistentPlayerData = playerData;
            _commonCards = commonCardsList;
            _firstTimeCardsListConfig = firstTimeCardsList;
        }

        public void InitializeAfterLoadSaves(CardsDatabase cardsDatabase)
        {
            // First time cards
            if (_persistentPlayerData.inventoryCards.Count == 0)
            {
                var set = new HashSet<string>();
                // init first time cards
                for (var i = 0; i < _firstTimeCardsListConfig.Cards.Count; i++)
                {
                    var cardConfig = _firstTimeCardsListConfig.Cards[i];
                    var cardSaveData = new CardSaveData
                    {
                        id = cardConfig.uniqueID,
                        level = 0,
                        amount = 0,
                        isOpen = true,
                        equippedAtSlot = i
                    };

                    var deckCard = new DeckCard
                    {
                        CardConfig = cardConfig,
                        CardSaveData = cardSaveData
                    };

                    _persistentPlayerData.inventoryCards.Add(cardSaveData);
                    _inventoryCards.Add(deckCard);
                    set.Add(cardConfig.uniqueID);
                }

                // init other buildings
                var allOtherBuildings = cardsDatabase.GetAllItems().Where(i => i.IsBuilding && !set.Contains(i.uniqueID));
                foreach (var cardConfig in allOtherBuildings)
                {
                    var cardSaveData = new CardSaveData
                    {
                        id = cardConfig.uniqueID,
                        level = 0,
                        amount = 0,
                        isOpen = false
                    };
                    
                    var deckCard = new DeckCard
                    {
                        CardConfig = cardConfig,
                        CardSaveData = cardSaveData
                    };
                    
                    _persistentPlayerData.inventoryCards.Add(cardSaveData);
                    _inventoryCards.Add(deckCard);
                }
                return;
            }
            
            foreach (var cardSaveData in _persistentPlayerData.inventoryCards)
            {
                var cardConfig = cardsDatabase.GetById(cardSaveData.id);
                _inventoryCards.Add(new DeckCard
                {
                    CardConfig = cardConfig,
                    CardSaveData = cardSaveData
                });
            }
        }

        public IEnumerable<CardConfig> GetCardsForGame()
        {
            var equipped = _inventoryCards
                .Where(i => i.CardSaveData.equippedAtSlot >= 0)
                .Select(i => i.CardConfig);
            return _commonCards.Cards.Concat(equipped);
        }
        
        public List<DeckCard> GetEquippedCards()
        {
            return _inventoryCards.Where(i => i.CardSaveData.equippedAtSlot >= 0).ToList();
        }

        public List<DeckCard> GetInventoryCards()
        {
            return _inventoryCards;
        }
    }
}