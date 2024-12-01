using System.Collections.Generic;
using Project.Runtime.Features;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Player;
using Project.Runtime.Player.Databases;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Services.PlayerProgress;
using Runtime.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Lobby.Equipment
{
    public class EquipmentPanel : PanelBase
    {
        [Inject] private PlayerDeck _playerDeck;
        [Inject] private CardsDatabase _cardsDatabase;
        [Inject] private PersistentPlayerData _persistentPlayerData;
        
        [SerializeField] private ItemInfoPopup itemInfoPopup;
        [SerializeField] private List<InventoryItemView> equipmentItemViews;
        [Title("Towers")]
        [SerializeField] private InventoryItemView equipmentItemViewPrefab;
        [SerializeField] private RectTransform towersContainer;
        
        [Title("Currencies for notification dots")]
        [SerializeField] private NotificationDot navNotificationDot;
        [SerializeField] private CurrencyConfig softCurrencyConfig;
        [SerializeField] private CurrencyConfig hexCurrencyConfig;

        private readonly List<InventoryItemView> _inventoryItemViews = new();
        
        private void Start()
        {
            // Inventory
            foreach (var deckCard in _playerDeck.GetInventoryCards())
            {
                var view = Instantiate(equipmentItemViewPrefab, towersContainer);
                view.SetDeckCardData(deckCard);
                view.notificationDot.SetActive(CanBeUpgraded(deckCard));
                view.OnClick += OnInventoryItemClick;
                _inventoryItemViews.Add(view);
            }
            
            // Equipment
            foreach (var equipmentItemView in equipmentItemViews)
            {
                equipmentItemView.OnClick += OnEquipmentItemClick;
            }
            
            _playerDeck.OnChangeEquipment += RefreshEquippedViews;
            RefreshEquippedViews();
            
            _persistentPlayerData.OnChangeWalletBalance += ChangeBalanceRefreshNotificationDot;
        }

        private void OnDestroy()
        {
            _playerDeck.OnChangeEquipment -= RefreshEquippedViews;
            _persistentPlayerData.OnChangeWalletBalance -= ChangeBalanceRefreshNotificationDot;
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
            Debug.Log("refresh equipped views");
            
            // Equipment
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
            
            // Inventory refresh
            foreach (var inventoryItemView in _inventoryItemViews)
            {
                inventoryItemView.SetDeckCardData(inventoryItemView.DeckCard);
            }
            
            RefreshNotificationDot();
        }

        private void ChangeBalanceRefreshNotificationDot(Wallet _) => RefreshNotificationDot();
        private void RefreshNotificationDot()
        {
            // Notification dot visibility
            var anyCanBeUpgraded = false;
            foreach (var view in _inventoryItemViews)
            {
                var canBeUpgraded = CanBeUpgraded(view.DeckCard);
                view.notificationDot.SetActive(canBeUpgraded);
                anyCanBeUpgraded = anyCanBeUpgraded || canBeUpgraded;
            }
            navNotificationDot.SetActive(anyCanBeUpgraded);
        }

        private bool CanBeUpgraded(DeckCard deckCard)
        {
            var amountToUpgrade = UpgradeConstants.GetCardAmountToUpgrade(deckCard);
            var softCurrencyCost = UpgradeConstants.GetUpgradeCostSoftCurrency(deckCard);
            var hexCurrencyCost = UpgradeConstants.GetUpgradeCostHexCurrency(deckCard);
            if (deckCard.CardSaveData.amount >= amountToUpgrade &&
                _persistentPlayerData.WalletByCurrency[softCurrencyConfig].Has((ulong)softCurrencyCost) &&
                _persistentPlayerData.WalletByCurrency[hexCurrencyConfig].Has((ulong)hexCurrencyCost)
               )
            {
                return true;
            }
            return false;
        }
    }
}