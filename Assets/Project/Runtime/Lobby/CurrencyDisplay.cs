using Project.Runtime.Player;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Services.PlayerProgress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby
{
    public class CurrencyDisplay : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image currencyIcon;
        [SerializeField] private CurrencyConfig currencyConfig;
        

        private Wallet _wallet;

        private void Start()
        {
            Subscribe(_persistentPlayerData.WalletByCurrency[currencyConfig]);
        }

        private void Subscribe(Wallet wallet)
        {
            _wallet = wallet;
            currencyIcon.sprite = wallet.CurrencyConfig.Icon;
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