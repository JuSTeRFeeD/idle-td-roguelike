using System;
using UnityEngine;

namespace Project.Runtime.Features.TimeManagement
{
    public static class TimeScale
    {
        private static float _normalTimeScale = 1f;
        
        public static void SetTimeScale(float timeScale)
        {
            if (timeScale < 0f) timeScale = 0f;
            if (timeScale > 2f) timeScale = 2f;
            
            Time.timeScale = timeScale;
            Debug.Log($"[TimeScale] SetTimeScale {timeScale}");
        }

        public static void SetNormalTimeScale()
        {
            Time.timeScale = _normalTimeScale;
        }

        public static void OverrideNormalTimeScale(float normalTimeScale)
        {
            if (normalTimeScale < 0f) normalTimeScale = 1;
            if (normalTimeScale > 2f) normalTimeScale = 2f;

            // Если в "нормальном" состоянии то меняем его
            if (Math.Abs(Time.timeScale - _normalTimeScale) < 0.01f)
            {
                SetTimeScale(normalTimeScale);
            }
            
            _normalTimeScale = normalTimeScale;
        }
    }
}