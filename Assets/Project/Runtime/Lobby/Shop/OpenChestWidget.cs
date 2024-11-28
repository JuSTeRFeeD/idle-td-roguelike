using System;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using Runtime.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.Shop
{
    public class OpenChestWidget : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;
        [Inject] private ChestOpeningController _chestOpeningController;

        [Title("Setup")]
        [SerializeField] private ChestType chestType;
        [SerializeField] private CurrencyConfig chestKeysCurrencyConfig;
        [SerializeField] private CurrencyConfig chestCurrencyConfig;
        [SerializeField] private DropChancesConfig dropChancesConfig;
        [Space]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button openButton;
        [SerializeField] private Button openX5Button;
        [SerializeField] private NotificationDot notificationDot;

        private void Start()
        {
            openButton.onClick.AddListener(() => Open(1));
            openX5Button.onClick.AddListener(() => Open(5));
            _persistentPlayerData.OnChangeWalletBalance += OnChangeWalletsBalance;
            RefreshCurrency();
        }

        private void OnDestroy()
        {
            _persistentPlayerData.OnChangeWalletBalance -= OnChangeWalletsBalance;
        }

        private void Open(ulong amount)
        {
            var chestWallet = _persistentPlayerData.GetWalletByCurrencyId(chestCurrencyConfig.uniqueID);
            var keysWallet = _persistentPlayerData.GetWalletByCurrencyId(chestKeysCurrencyConfig.uniqueID);

            if (chestWallet.Balance + keysWallet.Balance < amount) return;

            var estimate = amount;
            if (!chestWallet.Has(estimate))
            {
                if (chestWallet.Take(chestWallet.Balance))
                {
                    estimate -= chestWallet.Balance;
                    keysWallet.Take(estimate);
                } else return;
            }
            else
            {
                chestWallet.Take(amount);
            }
            
            // Open chest
            _chestOpeningController.OpenChest(chestType, dropChancesConfig, (int)amount);
        }

        private void OnChangeWalletsBalance(Wallet _) => RefreshCurrency();

        private void RefreshCurrency()
        {
            var chestsAmount = _persistentPlayerData.GetWalletByCurrencyId(chestCurrencyConfig.uniqueID).Balance;
            var keysAmount = _persistentPlayerData.GetWalletByCurrencyId(chestKeysCurrencyConfig.uniqueID).Balance;
            var amount = chestsAmount + keysAmount;
            string title;
            switch (chestType)
            {
                case ChestType.Common:
                    title = "Обычный сундук";
                    break;
                case ChestType.Epic:
                    title = "Эпический сундук";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            titleText.SetText($"{title}\n(Доступно {amount})");
            openButton.interactable = amount > 0;
            openX5Button.gameObject.SetActive(amount >= 5);  
            
            notificationDot.SetActive(amount > 0);
        }
    }
}