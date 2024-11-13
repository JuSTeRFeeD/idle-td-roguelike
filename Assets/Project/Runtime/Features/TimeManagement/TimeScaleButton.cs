using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Runtime.Features.TimeManagement
{
    public class TimeScaleButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI curTimeScaleText;

        private enum TimeScaleVariant
        {
            Normal = 0,
            Medium = 1,
            High = 2,
            Slow = 3,
        }

        private TimeScaleVariant _timeScaleVariant;
        
        private void Start()
        {
            _timeScaleVariant = TimeScaleVariant.Normal;
            curTimeScaleText.SetText("x1");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (_timeScaleVariant)
            {
                case TimeScaleVariant.Normal:
                    _timeScaleVariant = TimeScaleVariant.Medium;
                    TimeScale.OverrideNormalTimeScale(1.5f);
                    curTimeScaleText.SetText("x1.5");
                    break;
                case TimeScaleVariant.Medium:
                    _timeScaleVariant = TimeScaleVariant.High;
                    TimeScale.OverrideNormalTimeScale(2f);
                    curTimeScaleText.SetText("x2");
                    break;
                case TimeScaleVariant.High:
                    _timeScaleVariant = TimeScaleVariant.Slow;
                    TimeScale.OverrideNormalTimeScale(.5f);
                    curTimeScaleText.SetText("x0.5");
                    break;
                case TimeScaleVariant.Slow:
                    _timeScaleVariant = TimeScaleVariant.Normal;
                    TimeScale.OverrideNormalTimeScale(1f);
                    curTimeScaleText.SetText("x1");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}