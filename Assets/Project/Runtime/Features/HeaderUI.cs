using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Widgets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features
{
    public class HeaderUI : MonoBehaviour
    {
        [SerializeField] private AmountWidget woodAmountText;
        [SerializeField] private AmountWidget stoneAmountText;
        [SerializeField] private AmountWidget unitsAmountText;
        [Space]
        [SerializeField] private TextMeshProUGUI dayNightText;
        [Space] 
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private Image playerLevelExpFillImage;

        public void SetResourcesAmount(TotalResourcesData totalResourcesData)
        {
            woodAmountText.SetText($"{totalResourcesData.WoodAmount}/{totalResourcesData.WoodCapacity}");
            stoneAmountText.SetText($"{totalResourcesData.StoneAmount}/{totalResourcesData.StoneCapacity}");
        }

        public void SetUnitsAmount(TotalUnitsData totalUnitsData)
        {
            unitsAmountText.SetText(totalUnitsData.UsedUnitsAmount, totalUnitsData.TotalUnitsAmount);
        }

        public void SetDayNight(DayNight dayNight, bool isDay)
        {
            var dayOrNight = isDay ? "Day" : "Night";
            var time = dayNight.EstimateTime > 5f 
                ? $"{(int)dayNight.EstimateTime}" 
                : $"{dayNight.EstimateTime:#.0}";
            dayNightText.SetText($"{dayOrNight} {time} sec\n<size=80%>Day {dayNight.DayNumber}");
        }

        public void SetLevelExp(float current, float target)
        {
            playerLevelExpFillImage.fillAmount = current / target;
        }
        
        public void SetLevel(int level)
        {
            playerLevelText.SetText($"{level}\n<size=40%>LEVEL");
        }
    }
}