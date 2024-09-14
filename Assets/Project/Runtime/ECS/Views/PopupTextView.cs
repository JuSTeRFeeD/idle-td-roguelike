using TMPro;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class PopupTextView : EntityView
    {
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI valueBlackText;

        public void SetValue(float value)
        {
            valueText.SetText($"{value}");
            valueBlackText.SetText($"{value}");
        }
    }
}