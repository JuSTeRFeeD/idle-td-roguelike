using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.ProgressionRewards
{
    public class RewardsProgressView : MonoBehaviour
    {
        [SerializeField] private Slider sliderProgress;
        [Space]
        [SerializeField] private RewardItemView[] rewardItemViews;
        [SerializeField] private Reward[] rewards;

        private void Start()
        {
            for (var i = 0; i < rewards.Length; i++)
            {
                var reward = rewards[i];
                var view = rewardItemViews[i];
                view.Setup(reward);
            }
        }
    }
}