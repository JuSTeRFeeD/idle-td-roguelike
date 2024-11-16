using System;
using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Card;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
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
        private readonly CardsDatabase _cardsDatabase;
        private readonly ISaveManager _saveManager;

        private ActiveCardsListConfig _commonCards;
        private ActiveCardsListConfig _firstTimeCardsListConfig;

        private readonly List<DeckCard> _inventoryCards = new();
        
        public event Action OnChangeEquipment;

        [Inject]
        public PlayerDeck(PersistentPlayerData playerData, CardsDatabase cardsDatabase, ISaveManager saveManager)
        {
            _persistentPlayerData = playerData;
            _cardsDatabase = cardsDatabase;
            _saveManager = saveManager;
        }

        public void InitializeAfterLoadSaves(ActiveCardsListConfig commonCardsList, ActiveCardsListConfig firstTimeCardsList)
        {
            _commonCards = commonCardsList;
            _firstTimeCardsListConfig = firstTimeCardsList;
            var addedIds = new HashSet<string>();
            
            // First time cards
            if (_persistentPlayerData.InventoryCards.Count == 0)
            {
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

                    _persistentPlayerData.InventoryCards.Add(cardSaveData);
                    _inventoryCards.Add(deckCard);
                    addedIds.Add(cardConfig.uniqueID);
                }

                // init other buildings
                var allOtherBuildings = _cardsDatabase.GetAllItems().Where(i => i.IsBuilding && !addedIds.Contains(i.uniqueID));
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
                    
                    _persistentPlayerData.InventoryCards.Add(cardSaveData);
                    _inventoryCards.Add(deckCard);
                }
                return;
            }

            // adding from saves
            foreach (var cardSaveData in _persistentPlayerData.InventoryCards)
            {
                var cardConfig = _cardsDatabase.GetById(cardSaveData.id);
                if (!cardConfig) continue;
                _inventoryCards.Add(new DeckCard
                {
                    CardConfig = cardConfig,
                    CardSaveData = cardSaveData
                });
                addedIds.Add(cardConfig.uniqueID);
            }
            
            // добавление недостающих (новых после обнов)
            var notSavedOtherBuildings = _cardsDatabase.GetAllItems().Where(i => i.IsBuilding && !addedIds.Contains(i.uniqueID));
            foreach (var cardConfig in notSavedOtherBuildings)
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
                    
                _persistentPlayerData.InventoryCards.Add(cardSaveData);
                _inventoryCards.Add(deckCard);
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

        public void EquipCard(DeckCard deckCard, int toSlotIndex)
        {
            var equipped = _inventoryCards.Find(i => i.CardSaveData.equippedAtSlot == toSlotIndex);
            equipped.CardSaveData.equippedAtSlot = -1;
            deckCard.CardSaveData.equippedAtSlot = toSlotIndex;
            _saveManager.Save();
            OnChangeEquipment?.Invoke();
        }
    }
}