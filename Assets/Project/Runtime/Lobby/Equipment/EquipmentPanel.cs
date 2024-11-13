using System;
using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Features;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Lobby.Equipment
{
    public class EquipmentPanel : PanelBase
    {
        [Inject] private PlayerDeck _playerDeck;
        [Inject] private CardsDatabase _cardsDatabase;
        
        [SerializeField] private ItemInfoPopup itemInfoPopup;
        [SerializeField] private List<InventoryItemView> equipmentItemViews;
        [Title("Towers")]
        [SerializeField] private InventoryItemView equipmentItemViewPrefab;
        [SerializeField] private RectTransform towersContainer;

        private readonly List<InventoryItemView> _inventoryItemViews = new();
        
        private void Start()
        {
            // Inventory
            foreach (var deckCard in _playerDeck.GetInventoryCards())
            {
                var view = Instantiate(equipmentItemViewPrefab, towersContainer);
                view.SetDeckCardData(deckCard);
                _inventoryItemViews.Add(view);
                view.OnClick += OnInventoryItemClick;
            }
            
            // Equipment
            foreach (var equipmentItemView in equipmentItemViews)
            {
                equipmentItemView.OnClick += OnEquipmentItemClick;
            }
            
            _playerDeck.OnChangeEquipment += RefreshEquippedViews;
            RefreshEquippedViews();
        }

        private void OnDestroy()
        {
            _playerDeck.OnChangeEquipment -= RefreshEquippedViews;
        }

        private void OnInventoryItemClick(InventoryItemView inventoryItemView)
        {
            itemInfoPopup.SetDeckCard(inventoryItemView.DeckCard);
            itemInfoPopup.Show();
        }

        private void OnEquipmentItemClick(InventoryItemView inventoryItemView)
        {
            itemInfoPopup.SetDeckCard(inventoryItemView.DeckCard);
            itemInfoPopup.Show();
        }

        private void RefreshEquippedViews()
        {
            var list = _playerDeck.GetEquippedCards();
            var index = 0;
            for (; index < list.Count; index++)
            {
                var equippedCard = list[index];
                equipmentItemViews[index].SetDeckCardData(equippedCard);
            }
            for (; index < equipmentItemViews.Count; index++)
            {
                equipmentItemViews[index].Clear();
            }
        }
    }
}