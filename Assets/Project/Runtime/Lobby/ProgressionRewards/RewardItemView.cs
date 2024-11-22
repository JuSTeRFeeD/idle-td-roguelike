using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.ProgressionRewards
{
    public class RewardItemView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Image canBeCollectedImage;
        [SerializeField] private Image collectedImage;
        [Space]
        [SerializeField] private TextMeshProUGUI requirementCurrencyText;
        [SerializeField] private Image requirementCurrencyIcon;

        public event Action OnClick;
        
        public void Setup(Reward reward)
        {
            canBeCollectedImage.enabled = false;
            collectedImage.enabled = false;
            
            if (reward.requirement.IsCurrencyRequirement)
            {
                requirementCurrencyIcon.sprite = reward.requirement.currencyTuple.currencyConfig.Icon;
                requirementCurrencyText.SetText(reward.requirement.currencyTuple.amount.ToString());
            }
            else
            {
                requirementCurrencyText.SetText("not code for this");
                // requirementCurrencyText.SetText(reward.requirement.globalStatisticsType.ToString() + reward.requirement.target);
            }

            if (reward.isCurrencyReward)
            {
                amountText.SetText(reward.currencyTuple.amount.ToString());
                itemIcon.sprite = reward.currencyTuple.currencyConfig.Icon;
            }
        }

        public void SetCanBeCollected(bool canBeCollected)
        {
            canBeCollectedImage.enabled = canBeCollected;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke();
        }

        public void SetCollected(bool collected)
        {
            collectedImage.enabled = collected;
        }
    }
}