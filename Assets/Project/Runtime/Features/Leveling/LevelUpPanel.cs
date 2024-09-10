using System;
using System.Collections.Generic;
using DG.Tweening;
using Project.Runtime.ECS;
using Project.Runtime.Features.GameplayMenus;
using Project.Runtime.Features.Widgets;
using Project.Runtime.Scriptable.Card;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Project.Runtime.Features.Leveling
{
    public class LevelUpPanel : PanelBase
    {
        [Inject] private WorldSetup _worldSetup;
        
        [SerializeField] private List<CardWidget> cards = new();

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
        }

        private void CardSelected()
        {
            OnCardSelect?.Invoke(cards[_selectedCardId].CardConfig);
            Hide();
        }

        public override void Show()
        {
            _selectedCardId = -1;
            var activeCardsList = _worldSetup.ActiveCardsListConfig.Cards;
            var idx = 0;
            foreach (var card in cards)
            {
                var randomCardConfig = Random.Range(0, activeCardsList.Count);

                card.SetConfig(activeCardsList[randomCardConfig]);
                card.SetIsSelected(false);
                
                AnimateCardShow(card, idx++);
            }

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
