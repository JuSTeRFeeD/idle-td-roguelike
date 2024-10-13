using System;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
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
        [SerializeField] private CurrencyConfig commonChestCurrencyConfig;
        [SerializeField] private CurrencyConfig epicChestCurrencyConfig;
        [Space]
        [SerializeField] private ChestType chestType;
        [SerializeField] private DropChancesConfig dropChancesConfig;
        [Space]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button openButton;
        [SerializeField] private Button openX5Button;

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

        private void Open(int amount)
        {
            // Taking currency
            switch (chestType)
            {
                case ChestType.Common:
                    if (!_persistentPlayerData.GetWalletByCurrencyId(commonChestCurrencyConfig.uniqueID).Take(amount))
                    {
                        return;
                    }
                    break;
                case ChestType.Epic:
                    if (!_persistentPlayerData.GetWalletByCurrencyId(epicChestCurrencyConfig.uniqueID).Take(amount))
                    {
                        return;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Open chest
            _chestOpeningController.OpenChest(chestType, dropChancesConfig, amount);
        }

        private void OnChangeWalletsBalance(Wallet _) => RefreshCurrency();

        private void RefreshCurrency()
        {
            var amount = 0;
            string title;
            switch (chestType)
            {
                case ChestType.Common:
                    amount = _persistentPlayerData.GetWalletByCurrencyId(commonChestCurrencyConfig.uniqueID).Balance;
                    title = "Обычный сундук";
                    break;
                case ChestType.Epic:
                    amount = _persistentPlayerData.GetWalletByCurrencyId(epicChestCurrencyConfig.uniqueID).Balance;
                    title = "Эпический сундук";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            titleText.SetText($"{title}\n(Доступно {amount})");
            openButton.interactable = amount > 0;
            openX5Button.gameObject.SetActive(amount >= 5);            
        }
    }
}