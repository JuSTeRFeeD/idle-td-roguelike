using Project.Runtime.ECS.Components;
using TMPro;
using UnityEngine;

namespace Project.Runtime.Features.Widgets
{
    public class AmountWidget : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI amountText;

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