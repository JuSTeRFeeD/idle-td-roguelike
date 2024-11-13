using System;
using Project.Runtime.Features;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Missions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.Missions
{
    public class MissionItemView : MonoBehaviour
    {
        [SerializeField] private InventoryItemView reward;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Button collectButton;
        [SerializeField] private TextMeshProUGUI collectButtonText;
        
        public event Action<string> OnClickCollect;
        private string _missionId;
        
        private void Start()
        {
            collectButton.onClick.AddListener(() => OnClickCollect?.Invoke(_missionId));
        }

        public void Setup(MissionConfig missionConfig, long currentValue, long valueAtStart, bool completed, bool rewarded)
        {
            _missionId = missionConfig.uniqueID;
            reward.SetCurrencyData(missionConfig.RewardCurrency.currencyConfig, missionConfig.RewardCurrency.amount);

            // Title
            if (missionConfig.MissionType is GlobalStatisticsType.PlayedTimeSeconds)
            {
                titleText.SetText($"{missionConfig.MissionName} ({missionConfig.ValueToComplete / 60} мин)");
            }
            else
            {
                titleText.SetText($"{missionConfig.MissionName} ({missionConfig.ValueToComplete.FormatValue()})");
            }

            // Slider progress
            var targetValue = missionConfig.ValueToComplete;
            currentValue -= valueAtStart;
            if (currentValue > targetValue)
            {
                currentValue = targetValue;
            }
            progressSlider.value = completed ? 1f : (float)currentValue / (float)targetValue;
            if (missionConfig.MissionType is GlobalStatisticsType.PlayedTimeSeconds)
            {
                progressText.SetText(completed ? "Заберите награду" : $"{currentValue / 60}/{targetValue/60} мин");
            }
            else
            {
                progressText.SetText(completed ? "Заберите награду" : $"{currentValue.FormatValue()}/{targetValue.FormatValue()}");
            }
            
            collectButton.gameObject.SetActive(completed);
            collectButton.interactable = !rewarded;
            collectButtonText.SetText(rewarded ? "Получено!" : "Получить");
        }
    }
}