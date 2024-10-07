using System;
using DG.Tweening;
using Project.Runtime.Scriptable.Card;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Features.Widgets
{
    public class CardWidget : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI cardTitleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI shortInfoText;
        [Space]
        [SerializeField] private Image[] pointsImages;
        [SerializeField] private Sprite emptyPointSprite;
        [SerializeField] private Sprite filledPointSprite;

        public int Id { get; private set; }
        public CardConfig CardConfig { get; private set; }

        public event Action<int> OnClickCard;
        public event Action<CardWidget, PointerEventData> OnDragCard; 
        public event Action<CardWidget, PointerEventData> OnDragCardStart; 
        
        public void Init(int id)
        {
            Id = id;
        }

        public void SetConfig(CardConfig cardConfig)
        {
            CardConfig = cardConfig;
            iconImage.sprite = cardConfig.Icon;
            if (cardTitleText) cardTitleText.SetText(cardConfig.Title);
            if (shortInfoText) shortInfoText.SetText(cardConfig.ShortInfo);
        }

        public void SetDescription(string text)
        {
            if (descriptionText) descriptionText.SetText(text);
        }

        public void HidePoints()
        {
            if (pointsImages.Length == 0) return;
            foreach (var pointsImage in pointsImages)
            {
                pointsImage.enabled = false;
            }
        }
        public void SetPoints(int points)
        {
            if (pointsImages.Length == 0) return;
            var i = 0;
            for (; i < pointsImages.Length; i++)
            {
                pointsImages[i].enabled = true;
                pointsImages[i].sprite = points > i ? filledPointSprite : emptyPointSprite;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickCard?.Invoke(Id);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragCard?.Invoke(this, eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnDragCardStart?.Invoke(this, eventData);
        }
    }
}