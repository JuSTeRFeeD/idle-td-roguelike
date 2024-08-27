using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Project.Runtime.Features
{
    public class DayNightCycleEffects : MonoBehaviour
    {
        public Volume dayVolume;
        public Volume nightVolume;
        public float transitionDuration = 2f;
        
        private float _transitionProgress = 0f;
        private bool _isDay = true;

        public void SetTime(bool isDayTime)
        {
            if (_isDay == isDayTime) return;
            
            _isDay = !_isDay;
            StopAllCoroutines();
            StartCoroutine(Transition(
                _isDay ? nightVolume : dayVolume, 
                _isDay ? dayVolume : nightVolume));
        }

        private IEnumerator Transition(Volume currentProfile, Volume targetProfile)
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

        private static void InterpolateVolumes(Volume from, Volume to, float t)
        {
            from.weight = 1f - t;
            to.weight = t;
        }
    }
}