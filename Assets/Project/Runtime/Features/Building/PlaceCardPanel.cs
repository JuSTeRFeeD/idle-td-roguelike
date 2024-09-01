using System;
using Project.Runtime.Features.GameplayMenus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Features.Building
{
    public class PlaceCardPanel : PanelBase, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image cancelFrame;

        private readonly Color _baseColor = new(0, 0, 0, 0.9f);
        private readonly Color _hoverColor = new(1, 0, 0, 0.9f);
        private bool _isPointerOnFrame;
        public event Action OnCardUseCancel;
        
        private void Start()
        {
            OnPointerExit(null);
        }

        public override void Show()
        {
            cancelFrame.color = _baseColor;
            _isPointerOnFrame = false;
            base.Show();
        }

        private void Update()
        {
            if (!IsPanelActive) return;
            if (!_isPointerOnFrame) return;

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Cancel building");
                OnCardUseCancel?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            cancelFrame.color = _hoverColor;
            _isPointerOnFrame = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            cancelFrame.color = _baseColor;
            _isPointerOnFrame = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }
    }
}