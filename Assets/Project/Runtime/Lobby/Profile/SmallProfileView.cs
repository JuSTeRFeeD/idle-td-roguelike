using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Project.Runtime.Lobby.Profile
{
    public class SmallProfileView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Slider levelProgressSlider;

        private void Start()
        {
            if (YG2.player.auth)
            {
                usernameText.SetText(YG2.player.name);
            }
            else
            {
                usernameText.SetText("Гость");
            }
        }
    }
}