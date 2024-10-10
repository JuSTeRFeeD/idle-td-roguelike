using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Features.Navigation
{
    public class NavItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI labelText;

        private int _navIndex;
        private Vector3 _baseIconPos;
        private Sequence _sequence;
        private bool _selected;
        
        private const float AnimTime = 0.3f;
        
        public event Action<int> OnClick;
        
        public void Setup(int navIndex)
        {
            _navIndex = navIndex;
        }
        
        public void SetSelected(bool value)
        {
            _selected = value;
            backgroundImage.color = value
                ? new Color(0, 0, 0, 0.2f)
                : new Color(0, 0, 0, 0);
            
            if (_baseIconPos == Vector3.zero) _baseIconPos = iconImage.transform.localPosition;
            _sequence?.Kill(true);
            
            if (value)
            {
                _sequence = DOTween.Sequence()
                    .Append(iconImage.transform.DOScale(1.15f, AnimTime * .25f))
                    .Append(iconImage.transform.DOLocalMove(_baseIconPos, AnimTime))
                    .Join(labelText.DOFade(1f, AnimTime))
                    .SetLink(gameObject);
            }
            else
            {
                _sequence = DOTween.Sequence()
                    .Append(iconImage.transform.DOScale(.8f, AnimTime * .25f))
                    .Append(iconImage.transform.DOLocalMove(_baseIconPos + Vector3.down * 20, AnimTime))
                    .Join(labelText.DOFade(0f, AnimTime))
                    .SetLink(gameObject);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(_navIndex);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            backgroundImage.color = new Color(0, 0, 0, 0.3f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            backgroundImage.color = _selected
                ? new Color(0, 0, 0, 0.2f)
                : new Color(0, 0, 0, 0);
        }
    }
}