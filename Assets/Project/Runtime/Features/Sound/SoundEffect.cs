using NTC.Pool;
using UnityEngine;

namespace Runtime.Features.Sound
{
    public class SoundEffect : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        
        public void Play(AudioClip clip, float volume)
        {
            source.PlayOneShot(clip, volume);
            NightPool.Despawn(gameObject, clip.length + 0.1f);
        }
    }
}
