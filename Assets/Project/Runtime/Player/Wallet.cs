using System;
using Project.Runtime.Scriptable.Currency;

namespace Project.Runtime.Player
{
    public class Wallet
    {
        public int Balance { get; private set; }
        
        /// prev, new 
        public event Action<int, int> OnChange;

        public readonly CurrencyConfig CurrencyConfig;

        public Wallet(CurrencyConfig currencyConfig)
        {
            CurrencyConfig = currencyConfig;
        }
        
        public void Add(int amount)
        {
            Balance += amount;
            OnChange?.Invoke(Balance - amount, Balance);
        }

        public bool Has(int amount)
        {
            return Balance >= amount;
        }

        public bool Take(int amount)
        {
            if (!Has(amount)) return false;
            Balance -= amount;
            OnChange?.Invoke(Balance + amount, Balance);
            return true;
        }
    }
}