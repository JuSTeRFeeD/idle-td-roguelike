using System;
using Project.Runtime.Scriptable.Currency;

namespace Project.Runtime.Scriptable.Shop
{
    [Serializable]
    public struct CurrencyTuple
    {
        public int amount;
        public CurrencyConfig currencyConfig;
    }
}