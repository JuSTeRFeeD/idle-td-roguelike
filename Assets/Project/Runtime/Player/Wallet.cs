using System;

namespace Project.Runtime.Player
{
    public class Wallet
    {
        public int Balance { get; private set; }

        public Wallet()
        {
        }
        
        public void Init(int balance)
        {
            Balance = balance;
        }
        
        /// prev, new 
        public event Action<int, int> OnChange; 
        
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