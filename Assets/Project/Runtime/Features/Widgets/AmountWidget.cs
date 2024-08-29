using Project.Runtime.ECS.Components;
using Project.Runtime.Scriptable.Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features.Widgets
{
    public class AmountWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Image iconImage;

        public void OverrideIcon(ResourceType resourceType)
        {
            // todo change iconImage.sprite
        }
        
        public void SetText(string amount)
        {
            amountText.SetText(amount);
        }
        
        public void SetText(int cur, int max)
        {
            amountText.SetText($"{cur}/{max}");
        }
        
        public void SetText(WoodStorage storage)
        {
            amountText.SetText($"{storage.Current}/{storage.Max}");
        }
        
        public void SetText(StoneStorage storage)
        {
            amountText.SetText($"{storage.Current}/{storage.Max}");
        }
    }
}