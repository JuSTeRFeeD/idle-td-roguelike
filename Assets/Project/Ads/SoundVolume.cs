using UnityEngine;

namespace Ads
{
    public class SoundVolume : MonoBehaviour
    {
        public static bool IsSoundsActive { get; private set; }

        private void Start()
        {
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