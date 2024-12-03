using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Project.Runtime.Features.Tooltip
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Inject] private TooltipRenderer _tooltipRenderer;
        
        [TextArea]
        [SerializeField] private string tooltipText;

        private bool _isPointerDown;
        private float _holdTimer;
        
        private const float PointerHoldTime = 0.2f;

        private void Update()
        {
            if (_isPointerDown)
            {
                _holdTimer += Time.deltaTime;
                if (_holdTimer >= PointerHoldTime)
                {
                    _tooltipRenderer.ShowTooltip(tooltipText, transform as RectTransform);
                    _isPointerDown = false; // Сбрасываем, чтобы не повторять показ
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isPointerDown)
            {
                _tooltipRenderer.ShowTooltip(tooltipText, transform as RectTransform);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tooltipRenderer.HideTooltip();
            _isPointerDown = false;
            _holdTimer = 0f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
            _holdTimer = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
            _holdTimer = 0f;
        }
    }
}