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

        private void Start()
        {
            foreach (var inventoryItemView in equipmentViews)
            {
                inventoryItemView.OnClick += OnSelectSlot;
            }

            blackoutCloseButton.onClick.AddListener(Hide);
        }

        private void OnSelectSlot(InventoryItemView inventoryItemView)
        {
            Hide();
        }

        public override void Show()
        {
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

            base.Show();
        }
    }
}