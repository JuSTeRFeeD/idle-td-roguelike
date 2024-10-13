using System;
using Project.Runtime.Scriptable.Currency;

namespace Project.Runtime.Player
{
    public class Wallet
    {
        public ulong Balance { get; private set; } = 0;
        
        /// prev, new 
        public event Action<ulong, ulong> OnChange;

        public readonly CurrencyConfig CurrencyConfig;

        public Wallet(CurrencyConfig currencyConfig)
        {
            CurrencyConfig = currencyConfig;
        }
        
        public void Add(ulong amount)
        {
            Balance += amount;
            OnChange?.Invoke(Balance - amount, Balance);
        }

        public bool Has(ulong amount)
        {
            return Balance >= amount;
        }

        public bool Take(ulong amount)
        {
            if (!Has(amount)) return false;
            Balance -= amount;
            OnChange?.Invoke(Balance + amount, Balance);
            return true;
        }
    }
}