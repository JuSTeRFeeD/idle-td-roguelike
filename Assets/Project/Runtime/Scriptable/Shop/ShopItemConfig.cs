using UnityEngine;

namespace Project.Runtime.Scriptable.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/ShopItem")]
    public class ShopItemConfig : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private int amount;
        [Space]
        [SerializeField] private int discountPercent;
        [SerializeField] private int price;
        [SerializeField] private ShopPriceType priceType;
        [Space]
        [SerializeField] private ShopGiveOnBuy giveOnBuy;

        public ShopGiveOnBuy GiveOnBuy => giveOnBuy;
        public int Price => price;
        public int DiscountPercent => discountPercent;
        public ShopPriceType PriceType => priceType;
        public string Title => title;
        public int Amount => amount;
    }

    public enum ShopPriceType
    {
        HardCurrency,
        SoftCurrency,
        RealCurrency
    }

    public enum ShopGiveOnBuy
    {
        CommonChest,
        EpicChest,
        HardCurrency
    }
}