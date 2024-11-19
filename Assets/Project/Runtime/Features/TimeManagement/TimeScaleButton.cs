using System;
using Ads;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using YG;

namespace Project.Runtime.Features.TimeManagement
{
    public class TimeScaleButton : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private SoundVolume soundVolume;
        
        [SerializeField] private TextMeshProUGUI curTimeScaleText;
        [SerializeField] private GameObject rewardedWrapper;

        private enum TimeScaleVariant
        {
            Normal = 0,
            Medium = 1,
            High = 2,
            Slow = 3,
        }

        private TimeScaleVariant _timeScaleVariant;
        private bool IsAdWatched = false;
        
        private void Start()
        {
            _timeScaleVariant = TimeScaleVariant.Normal;
            curTimeScaleText.SetText("x1");

            YandexGame.RewardVideoEvent += RewardedWatched;
            YandexGame.ErrorVideoEvent += CancelledRewardedAd;
        }

        private void CancelledRewardedAd()
        {
            soundVolume.Silence(false);
            TimeScale.OverrideNormalTimeScale(1);
        }

        private void RewardedWatched(int rewardId)
        {
            if (rewardId == (int)RewardedAdIds.TimeScaleButton)
            {
                IsAdWatched = true;
                rewardedWrapper.gameObject.SetActive(false);
                CancelledRewardedAd();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsAdWatched)
            {
                TimeScale.OverrideNormalTimeScale(0);
                soundVolume.Silence(true);
                YandexGame.RewVideoShow((int)RewardedAdIds.TimeScaleButton);
                return;
            }
            
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