using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Project.Runtime.Features
{
    public class DayNightCycleEffects : MonoBehaviour
    {
        public Animator nightComingAnimator;
        
        public PostProcessVolume dayVolume;
        public PostProcessVolume nightVolume;
        public float transitionDuration = 2f;
        
        private float _transitionProgress = 0f;
        private bool _isDay = true;
        private static readonly int AnimPlay = Animator.StringToHash("play");

        private void Start()
        {
            nightComingAnimator.gameObject.SetActive(true);
        }

        public void SetTime(bool isDayTime)
        {
            if (_isDay == isDayTime) return;
            
            _isDay = !_isDay;
            
            if (!_isDay) nightComingAnimator.SetTrigger(AnimPlay);
            
            StopAllCoroutines();
            StartCoroutine(Transition(
            _isDay ? nightVolume : dayVolume, 
            _isDay ? dayVolume : nightVolume));
        }

        private IEnumerator Transition(PostProcessVolume currentProfile, PostProcessVolume targetProfile)
        {
            var elapsedTime = 0f;
        
            while (elapsedTime < transitionDuration)
            {
                _transitionProgress = elapsedTime / transitionDuration;
        
                InterpolateVolumes(currentProfile, targetProfile, _transitionProgress);
        
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            InterpolateVolumes(currentProfile, targetProfile, 1f); // Завершить интерполяцию
        }
        
        private static void InterpolateVolumes(PostProcessVolume from, PostProcessVolume to, float t)
        {
            from.weight = 1f - t;
            to.weight = t;
        }
    }
}