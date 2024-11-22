using DG.Tweening;
using Project.Runtime.Player;
using Project.Runtime.Scriptable.Currency;
using Project.Runtime.Services.PlayerProgress;
using Project.Runtime.Services.Saves;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Project.Runtime.Lobby.ProgressionRewards
{
    public class RewardsProgressView : MonoBehaviour
    {
        [Inject] private PersistentPlayerData _persistentPlayerData;
        [Inject] private ISaveManager _saveManager;
        
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

                var idx = i;
                view.OnClick += () =>
                {
                    OnClickReward(idx);
                };
            }

            sliderProgress.value = 0;
            _persistentPlayerData.OnChangeWalletBalance += OnChangeWalletBalance;
            OnChangeWalletBalance(_persistentPlayerData.WalletByCurrency[progressCurrencyConfig]);
        }

        private void OnClickReward(int i)
        {
            Debug.Log("Click Reward: " + i);
            
            var reward = rewards[i];
            if (!reward.requirement.IsCurrencyRequirement) return;
            Debug.Log("1");
            var currencyRequirement = reward.requirement.currencyTuple;
            if (currencyRequirement.currencyConfig != progressCurrencyConfig) return;
            Debug.Log("2");
            var wallet = _persistentPlayerData.WalletByCurrency[currencyRequirement.currencyConfig];
            if (!wallet.Has((uint)currencyRequirement.amount)) return;
            Debug.Log("3");
            if (_persistentPlayerData.DailyRewardProgressCollected[i]) return;
            _persistentPlayerData.DailyRewardProgressCollected[i] = true;
            Debug.Log("4");
            if (reward.isCurrencyReward)
            {
                _persistentPlayerData.WalletByCurrency[reward.currencyTuple.currencyConfig].Add((ulong)reward.currencyTuple.amount);
            }
            
            _saveManager.Save();
            
            rewardItemViews[i].SetCollected(true);
        }

        private void OnDestroy()
        {
            _persistentPlayerData.OnChangeWalletBalance -= OnChangeWalletBalance;
        }

        private void OnChangeWalletBalance(Wallet wallet)
        {
            if (wallet.CurrencyConfig != progressCurrencyConfig) return;

            var idx = 0;
            foreach (var reward in rewards)
            {
                if (reward.requirement.IsCurrencyRequirement)
                {
                    var currencyRequirement = reward.requirement.currencyTuple;
                    if (currencyRequirement.currencyConfig == wallet.CurrencyConfig)
                    {
                        if (wallet.Has((ulong)currencyRequirement.amount))
                        {
                            rewardItemViews[idx].SetCanBeCollected(true);
                            if (_persistentPlayerData.DailyRewardProgressCollected[idx])
                            {
                                rewardItemViews[idx].SetCollected(true);
                            }
                            idx++;
                            continue;
                        }
                        
                        rewardItemViews[idx].SetCanBeCollected(false);
                        rewardItemViews[idx].SetCollected(false);
                        idx++;
                        continue;
                    }
                }
                idx++;
            }
            sliderProgress.DOKill(true);
            sliderProgress.DOValue(wallet.Balance / 600f, 0.25f).SetLink(sliderProgress.gameObject);
        }
    }
}