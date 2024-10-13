using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.Widgets;
using Project.Runtime.Scriptable.Card;
using Runtime.Features.Widgets;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Features.Leveling
{
    public class LevelUpPanel : PanelBase
    {
        [Inject] private LevelUpCardsManager _levelUpCardsManager;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip levelUpSound;
        [SerializeField] private List<CardWidget> cards = new();

        public event Action<CardConfig> OnCardSelect; 
        
        private void Start()
        {
            Hide();

            var idx = 0;
            foreach (var card in cards)
            {
                card.Init(idx++);
                card.OnClickCard += OnCardClickCard;
            }
        }

        private void OnCardClickCard(int id)
        {
            var cardConfig = cards[id].CardConfig;
            OnCardSelect?.Invoke(cardConfig);
            _levelUpCardsManager.DecreaseDropCount(cardConfig);
            
            Hide();
        }

        private string GetDescription(CardConfig cardConfig)
        {
            var descriptionStringBuilder = new StringBuilder();
            var perksCount = cardConfig.Perks.Count;
            _levelUpCardsManager.AppliesCountByPerkUniqueId.TryGetValue(cardConfig.uniqueID, out var applyIndex);
            for (var index = 0; index < perksCount; index++)
            {
                var cardConfigPerk = cardConfig.Perks[index];
                descriptionStringBuilder.Append(cardConfigPerk.GetPerkDescription(applyIndex));
                if (index + 1 < perksCount) descriptionStringBuilder.Append("\n");
            }
            return descriptionStringBuilder.ToString();
        }
        
        public override void Show()
        {
            audioSource.PlayOneShot(levelUpSound);
            
            var idx = 0;
            var cardsToShow = _levelUpCardsManager.GetRandomCard();
            foreach (var card in cards)
            {
                if (idx < cardsToShow.Count && cardsToShow[idx])
                {
                    var config = cardsToShow[idx];
                    card.gameObject.SetActive(true);
                    card.SetConfig(config);
                    card.SetDescription(GetDescription(config));

                    if (config.IsBuilding)
                    {
                        card.HidePoints();
                    }
                    else
                    {
                        _levelUpCardsManager.AppliesCountByPerkUniqueId.TryGetValue(config.uniqueID, out var applyIndex);
                        card.SetPoints(applyIndex);
                    }
                    
                    AnimateCardShow(card, idx++);
                }
                else
                {
                    card.gameObject.SetActive(false);
                }
            }

            base.Show();
        }

        private static void AnimateCardShow(CardWidget cardWidget, int idx)
        {
            Transform cardTransform;
            (cardTransform = cardWidget.transform).DOKill();
            cardTransform.localScale = Vector3.zero;
            cardTransform
                .DOScale(1f, 0.25f)
                .SetDelay(idx * 0.1f)
                .SetLink(cardTransform.gameObject);
        }
    }
}
