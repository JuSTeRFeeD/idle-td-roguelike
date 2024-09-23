using System;
using DG.Tweening;
using Project.Runtime.Scriptable.Card;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Features.Widgets
{
    public class CardWidget : MonoBehaviour, IPointerClickHandler, IDragHandler
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI cardTitleText;
        [Space]
        [SerializeField] private Image[] pointsImages;
        [SerializeField] private Sprite emptyPointSprite;
        [SerializeField] private Sprite filledPointSprite;

        private Vector3 _initScale;
        private const float AnimDuration = 0.1f;

        public int Id { get; private set; }
        public CardConfig CardConfig { get; private set; }

        public event Action<int> OnClickCard;
        public event Action<CardWidget, PointerEventData> OnDragCard; 
        
        public void Init(int id)
        {
            Id = id;
        }

        public void SetConfig(CardConfig cardConfig)
        {
            CardConfig = cardConfig;
            iconImage.sprite = cardConfig.Icon;
            cardTitleText.SetText(cardConfig.Title);
        }

        public void HidePoints()
        {
            foreach (var pointsImage in pointsImages)
            {
                pointsImage.enabled = false;
            }
        }
        public void SetPoints(int points)
        {
            var i = 0;
            for (; i < pointsImages.Length; i++)
            {
                pointsImages[i].enabled = true;
                pointsImages[i].sprite = points > i ? filledPointSprite : emptyPointSprite;
            }
        }

        private void Start()
        {
            _initScale = bgImage.transform.localScale * .95f;
        }

        public void SetIsSelected(bool value)
        {
            bgImage.transform.DOKill();
            bgImage.transform
                .DOScale(value ? _initScale * 1.2f : _initScale, AnimDuration * Time.timeScale)
                .SetLink(gameObject);
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickCard?.Invoke(Id);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragCard?.Invoke(this, eventData);
        }
    }
}