using System;
using System.Collections.Generic;
using System.Linq;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Runtime.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using YG;

namespace Project.Runtime.Lobby.Shop
{
    public class ShopPanel : PanelBase
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;

        [SerializeField] private CurrencyConfig[] keysCurrencyConfigs;
        [SerializeField] private CurrencyConfig hardCurrencyConfig;
        [SerializeField] private CurrencyConfig commonChestCurrencyConfig;
        [SerializeField] private CurrencyConfig epicChestCurrencyConfig;
        [SerializeField] private List<ShopItemView> shopItemViews;

        [Title("Purchases")]
        [SerializeField] private ShopItemView shopItemView;
        [SerializeField] private RectTransform crystalsContainer;

        [Title("Purchases icons")] 
        [SerializeField] private Sprite crystalsIcon;
        
        [Title("Notifications")]
        [SerializeField] private NotificationDot notificationDot;
        
        private void Start()
        {
            foreach (var itemView in shopItemViews)
            {
                itemView.OnClick += OnClickShopItem;
            }

            InstantiatePurchases();
            
            _persistentPlayerData.OnChangeWalletBalance += OnChangeWalletBalance;
            DisplayDot();
        }

        private void OnDestroy()
        {
            _persistentPlayerData.OnChangeWalletBalance -= OnChangeWalletBalance;
        }

        private void OnChangeWalletBalance(Wallet _) => DisplayDot();

        private void DisplayDot()
        {
            if (keysCurrencyConfigs.Any(key => _persistentPlayerData.WalletByCurrency[key].Balance > 0) ||
                _persistentPlayerData.WalletByCurrency[commonChestCurrencyConfig].Balance > 0 || 
                _persistentPlayerData.WalletByCurrency[epicChestCurrencyConfig].Balance > 0)
            {
                notificationDot.SetActive(true);
                return;
            }
            notificationDot.SetActive(false);
        }

        private void InstantiatePurchases()
        {
            foreach (var purchase in YG2.purchases)
            {
                if (purchase.id.Contains("crystals"))
                {
                    var giveAmount = int.Parse(purchase.id.Split("_")[1]);
                    var item = Instantiate(shopItemView, crystalsContainer);
                    item.Setup(purchase.title, purchase.price, null, crystalsIcon, giveAmount, purchase.id);
                    item.OnClick += OnClickPurchase;
                }
            }
        }

        private void OnClickPurchase(ShopItemView item)
        {
            Debug.Log("ha");
            if (string.IsNullOrEmpty(item.PurchaseId)) return;
            Debug.Log("Click purchase " + item.PurchaseId);
            YG2.BuyPayments(item.PurchaseId);
        }

        private void OnClickShopItem(ShopItemView item)
        {
            if (!item.ShopItemConfig) return;
            var shopItemConfig = item.ShopItemConfig;
            var wallet = _persistentPlayerData.WalletByCurrency[shopItemConfig.PriceTuple.currencyConfig];
            if (!wallet.Take((uint)shopItemConfig.PriceTuple.amount))
            {
                return;
            }
            OnSuccessBuy(shopItemConfig);
        }

        private void OnSuccessBuy(ShopItemConfig shopItemConfig)
        {
            switch (shopItemConfig.GiveOnBuy)
            {
                case ShopGiveOnBuy.CommonChest:
                    _persistentPlayerData.WalletByCurrency[commonChestCurrencyConfig].Add((ulong)shopItemConfig.Amount);
                    break;
                case ShopGiveOnBuy.EpicChest:
                    _persistentPlayerData.WalletByCurrency[epicChestCurrencyConfig].Add((ulong)shopItemConfig.Amount);
                    break;
                case ShopGiveOnBuy.HardCurrency:
                    _persistentPlayerData.WalletByCurrency[hardCurrencyConfig].Add((ulong)shopItemConfig.Amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _saveManager.Save();
        }
    }
}