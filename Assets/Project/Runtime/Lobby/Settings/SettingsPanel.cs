using Ads;
using Project.Runtime.Features.GameplayMenus;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Runtime.Lobby.Settings
{
    public class SettingsPanel : PanelBase
    {
        [Inject] private SoundVolume soundVolume;
        
        [SerializeField] private Button closeButton;

        [Title("Sound Toggles")] 
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Toggle musicToggle;
        
        private void Start()
        {
            closeButton.onClick.AddListener(Hide);

            sfxToggle.isOn = SoundVolume.IsSfxActive;
            musicToggle.isOn = SoundVolume.IsMusicActive;
            
            sfxToggle.onValueChanged.AddListener(OnSfxToggle);
            musicToggle.onValueChanged.AddListener(OnMusicToggle);
        }

        private void OnSfxToggle(bool active)
        {
            soundVolume.SetSfxActive(active);
        }

        private void OnMusicToggle(bool active)
        {
            soundVolume.SetMusicActive(active);
        }
    }
}
