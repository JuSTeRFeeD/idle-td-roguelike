using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Widgets;
using TMPro;
using UnityEngine;

namespace Project.Runtime.Features
{
    public class HeaderUI : MonoBehaviour
    {
        [SerializeField] private AmountWidget woodAmountText;
        [SerializeField] private AmountWidget stoneAmountText;
        [SerializeField] private AmountWidget unitsAmountText;
        [Space]
        [SerializeField] private TextMeshProUGUI dayNightText;

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
    }
}