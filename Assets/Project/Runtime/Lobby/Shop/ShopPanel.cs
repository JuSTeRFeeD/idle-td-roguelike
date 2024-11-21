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
            foreach (var shopItemView in shopItemViews)
            {
                shopItemView.OnClick += OnClickShopItem;
            }

            InstantiatePurchases();
            
            _persistentPlayerData.OnChangeWalletBalance += OnChangeWalletBalance;
        }

        private void OnDestroy()
        {
            _persistentPlayerData.OnChangeWalletBalance -= OnChangeWalletBalance;
        }

        private void OnChangeWalletBalance(Wallet wallet)
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
                    var item = Instantiate(shopItemView, crystalsContainer);
                    item.Setup(purchase.title, purchase.priceValue, ShopPriceType.RealCurrency, crystalsIcon);
                }
            }
        }

        private void OnClickShopItem(ShopItemConfig shopItemConfig)
        {
#if UNITY_EDITOR
            OnSuccessBuy(shopItemConfig);
#endif
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