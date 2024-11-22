using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Runtime.Scriptable.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/ShopItem")]
    public class ShopItemConfig : ScriptableObject
    {
        [SerializeField] private string title;
        [SerializeField] private int amount;
        [PreviewField]
        [SerializeField] private Sprite icon;
        [Space]
        [SerializeField] private int discountPercent;
        [SerializeField] private CurrencyTuple priceTuple;
        [Space]
        [SerializeField] private ShopGiveOnBuy giveOnBuy;

        public ShopGiveOnBuy GiveOnBuy => giveOnBuy;
        public Sprite Icon => icon;
        public int DiscountPercent => discountPercent;
        public string Title => title;
        public int Amount => amount;
        public CurrencyTuple PriceTuple => priceTuple;
    }

    public enum ShopGiveOnBuy
    {
        CommonChest,
        EpicChest,
        HardCurrency
    }
}