using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features.Widgets
{
    public class UnitIconWidget : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Sprite filledSprite;
        [SerializeField] private Sprite unfilledSprite;
        [SerializeField] private Image lockedLevelBg;
        [SerializeField] private TextMeshProUGUI lockedLevelText;

        public enum UnitWidgetType
        {
            Unfilled,
            Filled,
            Locked,
            Hidden
        }

        public void SetType(UnitWidgetType unitWidgetType)
        {
            iconImage.enabled = true;
            iconImage.color = Color.white;
            lockedLevelBg.enabled = lockedLevelText.enabled = false;
            
            switch (unitWidgetType)
            {
                case UnitWidgetType.Unfilled:
                    iconImage.sprite = unfilledSprite;
                    break;
                case UnitWidgetType.Filled:
                    iconImage.sprite = filledSprite;
                    break;
                case UnitWidgetType.Locked:
                    iconImage.sprite = filledSprite;
                    lockedLevelBg.enabled = lockedLevelText.enabled = true;
                    iconImage.color = Color.gray;
                    break;
                case UnitWidgetType.Hidden:
                    iconImage.enabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(unitWidgetType), unitWidgetType, null);
            }
        }
    }
}