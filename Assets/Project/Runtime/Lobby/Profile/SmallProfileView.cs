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
            if (YandexGame.auth)
            {
                usernameText.SetText(YandexGame.playerName);
            }
            else
            {
                usernameText.SetText("Неизвестный путешественник");
            }
        }
    }
}