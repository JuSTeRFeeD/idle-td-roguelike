using System;
using Project.Runtime.Scriptable.Shop;
using Project.Runtime.Services.PlayerProgress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.Shop
{
    public class OpenChestWidget : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ChestOpeningController _chestOpeningController;
        
        [SerializeField] private ChestType chestType;
        [SerializeField] private Button openButton;
        [SerializeField] private TextMeshProUGUI openButtonText;

        private void Start()
        {
            openButton.onClick.AddListener(Open);
            _persistentPlayerData.Chests.OnChange += Refresh;
            Refresh();
        }

        private void Open()
        {
            if (_persistentPlayerData.Chests.TakeChest(chestType))
            {
                _chestOpeningController.OpenChest(chestType);
            }
            else
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            var amount = 0;
            switch (chestType)
            {
                case ChestType.Common:
                    amount = _persistentPlayerData.Chests.CommonChestCount;
                    break;
                case ChestType.Epic:
                    amount = _persistentPlayerData.Chests.EpicChestCount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            openButton.interactable = amount > 0;
            if (amount <= 0)
            {
                openButtonText.SetText("Недостаточно сундуков");
            }
            openButtonText.SetText($"Открыть (доступно {amount})");
        }
    }
}