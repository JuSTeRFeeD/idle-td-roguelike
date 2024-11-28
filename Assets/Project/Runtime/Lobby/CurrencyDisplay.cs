using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using Project.Runtime.Features;
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
        private Coroutine _routine;

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

        private void OnChangeBalance(ulong prevBalance, ulong newBalance)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(UpdateBalance(newBalance));
        }

        private IEnumerator UpdateBalance(ulong newBalance)
        {
            valueText.transform.DOKill(true);
            valueText.transform.DOPunchScale(Vector3.one * .5f, 0.2f, 1, 1);
            for (var i = 0; i < 15; i++)
            {
                valueText.SetText($"{Random.Range(0, 999):###}");
                yield return null;
            }
            valueText.SetText(newBalance.FormatValue());
        }
    }
}