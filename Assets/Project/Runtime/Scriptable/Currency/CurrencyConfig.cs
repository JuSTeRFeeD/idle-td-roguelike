using UnityEngine;

namespace Project.Runtime.Scriptable.Currency
{
    [CreateAssetMenu(menuName = "Game/NewCurrency")]
    public class CurrencyConfig : UniqueConfig
    {
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private string currencyName;
        [SerializeField] private int initialBalance;
        [SerializeField] private Rarity rarity;

        public string CurrencyName => currencyName;
        public Sprite Icon => iconSprite;
        public int InitialBalance => initialBalance;
        public Rarity Rarity =>  rarity;
    }
}