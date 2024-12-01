using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.TimeManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features
{
    public class PauseToSettings : MonoBehaviour
    {
        [SerializeField] private Button pauseSettingsButton;
        [SerializeField] private PanelBase settingsPanel;

        private void Start()
        {
            pauseSettingsButton.onClick.AddListener(PauseSettings);
        }

        private void PauseSettings()
        {
            TimeScale.SetTimeScale(0f);
            settingsPanel.Show();
        }

        public void Resume()
        {
            TimeScale.SetNormalTimeScale();
        }
    }
}
