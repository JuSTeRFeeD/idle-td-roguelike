using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features.Widgets
{
    public class TowerWidget : MonoBehaviour
    {
        [SerializeField] private Button upgradeButton;
        [SerializeField] private AmountWidget stoneAmountWidget;
        [SerializeField] private AmountWidget woodAmountWidget;

        public event Action OnClickUpgrade;

        private int _woodPrice;
        private int _stonePrice;
        
        private void Start()
        {
            upgradeButton.interactable = false;
            upgradeButton.onClick.AddListener(() => OnClickUpgrade?.Invoke());
        }

        public void SetPrices(int woodAmount, int stoneAmount)
        {
            _woodPrice = woodAmount;
            _stonePrice = stoneAmount;
            woodAmountWidget.SetText($"{woodAmount}");
            stoneAmountWidget.SetText($"{stoneAmount}");
        }
        
        public void SetResourcesInStorages(int woodAmount, int stoneAmount)
        {
            upgradeButton.interactable = woodAmount >= _woodPrice && stoneAmount >= _stonePrice;
        }
    }
}
