using System;
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
        [SerializeField] private ChestType chestType;
        [SerializeField] private ChestDropConfig chestDropConfig;
        [Space]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button openButton;
        [SerializeField] private Button openX5Button;

        private void Start()
        {
            openButton.onClick.AddListener(() => Open(1));
            openX5Button.onClick.AddListener(() => Open(5));
            _persistentPlayerData.Chests.OnChange += Refresh;
            Refresh();
        }

        private void Open(int amount)
        {
            if (_persistentPlayerData.Chests.TakeChest(chestType, amount))
            {
                _chestOpeningController.OpenChest(chestType, chestDropConfig, amount);
            }
            else
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            var amount = 0;
            string title;
            switch (chestType)
            {
                case ChestType.Common:
                    amount = _persistentPlayerData.Chests.CommonChestCount;
                    title = "Обычный сундук";
                    break;
                case ChestType.Epic:
                    amount = _persistentPlayerData.Chests.EpicChestCount;
                    title = "Эпический сундук";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            titleText.SetText($"{title} (Доступно {amount})");
            openButton.interactable = amount > 0;
            openX5Button.gameObject.SetActive(amount >= 5);            
        }
    }
}