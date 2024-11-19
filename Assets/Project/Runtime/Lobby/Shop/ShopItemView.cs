using System;
using Project.Runtime.Scriptable.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.Shop
{
    public class ShopItemView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ShopItemConfig shopItemConfig;
        [Space]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image currencyImage;
        [Space]
        [SerializeField] private TextMeshProUGUI discountText;
        [SerializeField] private Image discountImage;
        [Space]
        [SerializeField] private Sprite softCurrencySprite;
        [SerializeField] private Sprite hardCurrencySprite;
        [SerializeField] private Sprite realCurrencySprite;

        public event Action<ShopItemConfig> OnClick;
        
        private void Start()
        {
            if (!shopItemConfig) return;
            itemIcon.sprite = shopItemConfig.Icon;
            titleText.SetText($"{shopItemConfig.Title} {shopItemConfig.Amount}");
            priceText.SetText($"{shopItemConfig.Price}");
            SetCurrencyIcon(shopItemConfig.PriceType);

            discountText.SetText($"-{shopItemConfig.DiscountPercent}%");
            discountText.enabled = shopItemConfig.DiscountPercent != 0;
            discountImage.enabled = shopItemConfig.DiscountPercent != 0;
        }

        public void Setup(string title, string price, ShopPriceType priceType, Sprite icon)
        {
            itemIcon.sprite = icon;
            titleText.SetText(title);
            priceText.SetText(price);
            SetCurrencyIcon(priceType);
            
            discountText.enabled = discountImage.enabled = false;
        }

        private void SetCurrencyIcon(ShopPriceType priceType)
        {
            currencyImage.sprite = priceType switch
            {
                ShopPriceType.HardCurrency => hardCurrencySprite,
                ShopPriceType.SoftCurrency => softCurrencySprite,
                ShopPriceType.RealCurrency => realCurrencySprite,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(shopItemConfig);
        }
    }
}
