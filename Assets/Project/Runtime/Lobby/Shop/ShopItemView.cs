using System;
using Project.Runtime.Scriptable.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.Shop
{
    public class ShopItemView : MonoBehaviour
    {
        [SerializeField] private Button buyButton;
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

        public event Action<ShopItemView> OnClick;

        public ShopItemConfig ShopItemConfig => shopItemConfig;
        public int GiveHardAmount { get; private set; }
        
        private void Start()
        {
            if (!shopItemConfig) return;
            itemIcon.sprite = shopItemConfig.Icon;
            titleText.SetText($"{shopItemConfig.Title} {shopItemConfig.Amount}");
            priceText.SetText($"{shopItemConfig.PriceTuple.amount}");
            currencyImage.sprite = shopItemConfig.PriceTuple.currencyConfig.Icon;

            discountText.SetText($"-{shopItemConfig.DiscountPercent}%");
            discountText.enabled = shopItemConfig.DiscountPercent != 0;
            discountImage.enabled = shopItemConfig.DiscountPercent != 0;
            
            buyButton.onClick.AddListener(() => OnClick?.Invoke(this));
        }

        public void Setup(string title, string price, Sprite currencyIcon, Sprite icon, int giveAmount)
        {
            GiveHardAmount = giveAmount;
            itemIcon.sprite = icon;
            titleText.SetText(title);
            priceText.SetText(price);
            currencyImage.sprite = currencyIcon;
            currencyImage.enabled = currencyIcon;
            if (!currencyIcon)
            {
                priceText.rectTransform.offsetMin = new Vector2(
                    priceText.rectTransform.offsetMin.x - 32, 
                    priceText.rectTransform.offsetMin.y
                );
            }
            
            discountText.enabled = discountImage.enabled = false;
        }
    }
}
