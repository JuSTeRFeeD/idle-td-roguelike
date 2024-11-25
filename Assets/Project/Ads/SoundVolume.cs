using UnityEngine;
using UnityEngine.Audio;

namespace Ads
{
    public class SoundVolume : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        
        public static bool IsSoundsActive { get; private set; }
        public static bool IsSfxActive { get; private set; } = true;
        public static bool IsMusicActive { get; private set; } = true;

        
        private void Start()
        {
            if (PlayerPrefs.HasKey("SfxActive"))
            {
                IsSfxActive = PlayerPrefs.GetInt("SfxActive") == 1;
                SetSfxActive(IsSfxActive);
            }
            else IsSfxActive = true;
            
            if (PlayerPrefs.HasKey("MusicActive"))
            {
                IsMusicActive = PlayerPrefs.GetInt("MusicActive") == 1;
                SetMusicActive(IsMusicActive);
            }
            else IsMusicActive = true;
            
            if (PlayerPrefs.HasKey("SoundsActive"))
            {
                IsSoundsActive = PlayerPrefs.GetInt("SoundsActive") == 1;
                SetSoundActive(IsSoundsActive);
            }
            else
            {
                SetSoundActive(true);
            }
        }

        public void Silence(bool silence)
        {
            AudioListener.pause = silence;
            AudioListener.volume = silence ? 0 : 1;
        }
        
        public void SetSoundActive(bool value)
        {
            IsSoundsActive = value;
            PlayerPrefs.SetInt("SoundsActive", value ? 1 : 0);
            Silence(!value);
        }

        public void SetSfxActive(bool value)
        {
            IsSfxActive = value;
            mixer.SetFloat("SFX", value ? 0 : -80);
            PlayerPrefs.SetInt("SfxActive", value ? 1 : 0);
        }
        
        public void SetMusicActive(bool value)
        {
            IsMusicActive= value;
            mixer.SetFloat("Music", value ? -10 : -80);
            PlayerPrefs.SetInt("MusicActive", value ? 1 : 0);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            Silence(!hasFocus);
        }

        private void OnApplicationPause(bool isPaused)
        {
            Silence(isPaused);
        }
    }
}