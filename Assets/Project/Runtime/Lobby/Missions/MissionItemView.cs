using Project.Runtime.Features;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.Missions
{
    public class MissionItemView : MonoBehaviour
    {
        [SerializeField] private InventoryItemView reward;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Button collectButton;

    }
}