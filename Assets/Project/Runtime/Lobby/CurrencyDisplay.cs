using System;
using Project.Runtime.Player;
using Project.Runtime.Services.PlayerProgress;
using TMPro;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Lobby
{
    public class CurrencyDisplay : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private SubscribeToWalletType subscribeToWalletType;
        
        private enum SubscribeToWalletType
        {
            HardCurrency,
            SoftCurrency
        }

        private Wallet _wallet;

        private void Start()
        {
            switch (subscribeToWalletType)
            {
                case SubscribeToWalletType.HardCurrency:
                    Subscribe(_persistentPlayerData.HardCurrency);
                    break;
                case SubscribeToWalletType.SoftCurrency:
                    Subscribe(_persistentPlayerData.SoftCurrency);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Subscribe(Wallet wallet)
        {
            _wallet = wallet;
            OnChangeBalance(_wallet.Balance, _wallet.Balance);
            _wallet.OnChange += OnChangeBalance;
        }

        private void OnDestroy()
        {
            if (_wallet != null) _wallet.OnChange -= OnChangeBalance;
        }

        private void OnChangeBalance(int prevBalance, int newBalance)
        {
            valueText.SetText(newBalance.ToString());
        }
    }
}