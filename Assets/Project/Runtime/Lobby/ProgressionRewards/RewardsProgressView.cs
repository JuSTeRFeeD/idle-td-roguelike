using DG.Tweening;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Services.PlayerProgress;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.ProgressionRewards
{
    public class RewardsProgressView : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        
        [SerializeField] private CurrencyConfig progressCurrencyConfig;
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

            sliderProgress.value = 0;
            _persistentPlayerData.OnChangeWalletBalance += OnChangeWalletBalance;
            OnChangeWalletBalance(_persistentPlayerData.WalletByCurrency[progressCurrencyConfig]);
        }

        private void OnDestroy()
        {
            _persistentPlayerData.OnChangeWalletBalance -= OnChangeWalletBalance;
        }

        private void OnChangeWalletBalance(Wallet wallet)
        {
            if (wallet.CurrencyConfig != progressCurrencyConfig) return;

            foreach (var reward in rewards)
            {
                if (reward.requirement.IsCurrencyRequirement)
                {
                    var currencyRequirement = reward.requirement.currencyTuple;
                    if (currencyRequirement.currencyConfig != wallet.CurrencyConfig) continue;
                    if (wallet.Has((ulong)currencyRequirement.amount))
                    {
                        sliderProgress.DOKill(true);
                        sliderProgress.DOValue(reward.progressSliderValue, 0.25f).SetLink(sliderProgress.gameObject);
                    }
                }
            }
        }
    }
}