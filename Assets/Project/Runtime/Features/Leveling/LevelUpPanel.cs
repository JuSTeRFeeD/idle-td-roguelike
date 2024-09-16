using System;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.Widgets;
using Project.Runtime.Scriptable.Card;
using TMPro;
using UnityEngine;
using VContainer;

namespace Project.Runtime.Features.Leveling
{
    public class LevelUpPanel : PanelBase
    {
        [Inject] private LevelUpCardsManager _levelUpCardsManager;
        
        [SerializeField] private List<CardWidget> cards = new();
        [SerializeField] private TextMeshProUGUI selectedCardDescriptionText;
        
        public event Action<CardConfig> OnCardSelect; 
        private int _selectedCardId = -1;
        
        private void Start()
        {
            Hide();

            var idx = 0;
            foreach (var card in cards)
            {
                card.SetIsSelected(false);
                card.Init(idx++);
                card.OnClickCard += OnCardClickCard;
            }
        }

        private void OnCardClickCard(int id)
        {
            if (_selectedCardId == id)
            {
                CardSelected();
                return;
            }
            
            if (_selectedCardId >= 0)
            {
                cards[_selectedCardId].SetIsSelected(false);
            }
            _selectedCardId = id;
            cards[_selectedCardId].SetIsSelected(true);

            ShowSelectedCardDescription();
        }

        private void ShowSelectedCardDescription()
        {
            var descriptionStringBuilder = new StringBuilder();
            var perksCount = cards[_selectedCardId].CardConfig.Perks.Count;
            _levelUpCardsManager.AppliesCountByPerkUniqueId.TryGetValue(cards[_selectedCardId].CardConfig.uniqueID, out var applyIndex);
            for (var index = 0; index < perksCount; index++)
            {
                var cardConfigPerk = cards[_selectedCardId].CardConfig.Perks[index];
                descriptionStringBuilder.Append(cardConfigPerk.GetPerkDescription(applyIndex));
                if (index + 1 < perksCount) descriptionStringBuilder.Append("\n");
            }
            selectedCardDescriptionText.SetText(descriptionStringBuilder.ToString());
        }

        private void CardSelected()
        {
            var cardConfig = cards[_selectedCardId].CardConfig;
            OnCardSelect?.Invoke(cardConfig);
            _levelUpCardsManager.DecreaseDropCount(cardConfig);
            Hide();
        }

        public override void Show()
        {
            _selectedCardId = -1;
            var idx = 0;
            var randomCards = _levelUpCardsManager.GetRandomCard();
            foreach (var card in cards)
            {
                if (idx < randomCards.Count && randomCards[idx])
                {
                    card.gameObject.SetActive(true);
                    card.SetConfig(randomCards[idx]);
                    card.SetIsSelected(false);
                    
                    AnimateCardShow(card, idx++);
                }
                else
                {
                    card.gameObject.SetActive(false);
                }
            }

            OnCardClickCard(1);
            base.Show();
        }

        private static int AnimateCardShow(CardWidget cardWidget, int idx)
        {
            Transform cardTransform;
            (cardTransform = cardWidget.transform).DOKill();
            cardTransform.localScale = new Vector3(0, 1, 1);
            cardTransform
                .DOScaleX(1f, 0.235f)
                .SetDelay(idx * 0.2f)
                .SetLink(cardTransform.gameObject);
            return idx;
        }
    }
}
