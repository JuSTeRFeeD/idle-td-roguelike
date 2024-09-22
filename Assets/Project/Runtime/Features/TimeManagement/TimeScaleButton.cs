using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Runtime.Features.TimeManagement
{
    public class TimeScaleButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI curTimeScaleText;

        private enum Variants
        {
            Normal = 0,
            TheChildCant = 1,
            Faster = 2,
            FlashBoy = 3
        }

        private Variants _variant;
        
        private void Start()
        {
            _variant = Variants.Normal;
            curTimeScaleText.SetText("x1");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (_variant)
            {
                case Variants.Normal:
                    _variant = Variants.TheChildCant;
                    TimeScale.OverrideNormalTimeScale(0.5f);
                    curTimeScaleText.SetText("x0.5");
                    break;
                case Variants.TheChildCant:
                    _variant = Variants.Faster;
                    TimeScale.OverrideNormalTimeScale(1.5f);
                    curTimeScaleText.SetText("x0.5");
                    break;
                case Variants.Faster:
                    _variant = Variants.FlashBoy;
                    TimeScale.OverrideNormalTimeScale(2f);
                    curTimeScaleText.SetText("x2");
                    break;
                case Variants.FlashBoy:
                    _variant = Variants.Normal;
                    TimeScale.OverrideNormalTimeScale(1f);
                    curTimeScaleText.SetText("x1");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}