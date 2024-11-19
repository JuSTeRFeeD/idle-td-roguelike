using System.Collections.Generic;
using Project.Runtime.Features;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Player;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.Equipment
{
    public class SelectSlotToEquipPopup : PanelBase
    {
        [Inject] private PlayerDeck _playerDeck;

        [SerializeField] private Button blackoutCloseButton;
        [SerializeField] private List<InventoryItemView> equipmentViews;

        private DeckCard _deckCard;
        
        private void Start()
        {
            var idx = 0;
            foreach (var inventoryItemView in equipmentViews)
            {
                var slotIndex = idx;
                inventoryItemView.OnClick += (_) =>
                {
                    OnSelectSlot(slotIndex);
                };
                idx++;
            }

            blackoutCloseButton.onClick.AddListener(Hide);
        }

        private void OnSelectSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex > equipmentViews.Count - 1)
            {
                Hide();
                return;
            }

            _playerDeck.EquipCard(_deckCard, slotIndex);
            Hide();
        }

        public void Show(DeckCard deckCard)
        {
            _deckCard = deckCard;
            
            var list = _playerDeck.GetEquippedCards();
            var index = 0;
            for (; index < list.Count; index++)
            {
                var equippedCard = list[index];
                equipmentViews[index].SetDeckCardData(equippedCard);
            }

            for (; index < equipmentViews.Count; index++)
            {
                equipmentViews[index].Clear();
            }

            Show();
        }
    }
}