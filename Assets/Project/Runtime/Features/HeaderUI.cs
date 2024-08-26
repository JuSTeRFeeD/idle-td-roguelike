using Project.Runtime.ECS.Components;
using Project.Runtime.Features.Widgets;
using UnityEngine;

namespace Project.Runtime.Features
{
    public class HeaderUI : MonoBehaviour
    {
        [SerializeField] private AmountWidget woodAmountText;
        [SerializeField] private AmountWidget stoneAmountText;
        [SerializeField] private AmountWidget unitsAmountText;

        public void SetResourcesAmount(TotalResourcesData totalResourcesData)
        {
            woodAmountText.SetText($"{totalResourcesData.WoodAmount}/{totalResourcesData.WoodCapacity}");
            stoneAmountText.SetText($"{totalResourcesData.StoneAmount}/{totalResourcesData.StoneCapacity}");
        }

        public void SetUnitsAmount(TotalUnitsData totalUnitsData)
        {
            unitsAmountText.SetText(totalUnitsData.UsedUnitsAmount, totalUnitsData.TotalUnitsAmount);
        }
    }
}