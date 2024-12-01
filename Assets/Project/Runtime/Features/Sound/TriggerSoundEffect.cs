using System;
using NTC.Pool;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Features.Sound
{
    public class TriggerSoundEffect : MonoBehaviour
    {
        [SerializeField] private SoundEffect soundEffectPrefab;
        [SerializeField] private AudioClip clip;
        [MinValue(0), MaxValue(1)]
        [SerializeField] private float volume = 1f;

        private void OnEnable()
        {
            var sfx = NightPool.Spawn(soundEffectPrefab, transform.position, Quaternion.identity);
            sfx.Play(clip, volume);
        }
    }
}