using System;
using System.Collections.Generic;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Lobby.Shop
{
    public class ShopPanel : PanelBase
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;

        [SerializeField] private CurrencyConfig hardCurrencyConfig;
        [SerializeField] private CurrencyConfig commonChestCurrencyConfig;
        [SerializeField] private CurrencyConfig epicChestCurrencyConfig;
        [SerializeField] private List<ShopItemView> shopItemViews;
        
        private void Start()
        {
            foreach (var shopItemView in shopItemViews)
            {
                shopItemView.OnClick += OnClickShopItem;
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