using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Runtime.Features.Tooltip
{
    public class TooltipRenderer : MonoBehaviour
    {
        [SerializeField] private RectTransform tooltipRectTransform; // RectTransform тултипа
        [SerializeField] private TextMeshProUGUI tooltipText; // Текст тултипа
        [SerializeField] private Canvas canvas; // Canvas, к которому привязан тултип
        [SerializeField] private Vector2 tooltipOffset = new Vector2(10f, 10f); // Отступ от мышки
        [SerializeField] private Camera uiCamera;

        private void Awake()
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                uiCamera = canvas.worldCamera;
            HideTooltip();
        }

        public void ShowTooltip(string text, RectTransform targetRect)
        {
            tooltipText.text = text;
            tooltipRectTransform.gameObject.SetActive(true);

            UpdatePosition(targetRect);
        }

        public void HideTooltip()
        {
            tooltipRectTransform.gameObject.SetActive(false);
        }

        private void UpdatePosition(RectTransform targetRect)
        {
            Vector3[] corners = new Vector3[4];
            targetRect.GetWorldCorners(corners); // Получаем углы элемента в мировых координатах

            // Считаем центр элемента
            Vector3 targetPosition = (corners[0] + corners[2]) / 2;

            // Переводим мировые координаты в экранные
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, targetPosition);

            // Переводим экранные координаты в локальные координаты Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPoint,
                uiCamera,
                out Vector2 localPoint);

            // Определяем направление от элемента к центру экрана
            Vector2 canvasCenter = Vector2.zero; // Центр Canvas
            Vector2 directionToCenter = (canvasCenter - localPoint).normalized;

            // Устанавливаем позицию с учетом отступа в сторону центра и не перекрывая элемент
            float offsetDistance = Mathf.Max(targetRect.rect.width, targetRect.rect.height) / 2 + tooltipOffset.magnitude;
            Vector2 targetTooltipPosition = localPoint + directionToCenter * offsetDistance;

            // Ограничиваем тултип внутри границ Canvas
            RectTransform canvasRect = canvas.transform as RectTransform;
            Vector2 canvasSize = canvasRect.sizeDelta;
            Vector2 tooltipSize = tooltipRectTransform.sizeDelta;

            float clampedX = Mathf.Clamp(targetTooltipPosition.x, -canvasSize.x / 2 + tooltipSize.x / 2, canvasSize.x / 2 - tooltipSize.x / 2);
            float clampedY = Mathf.Clamp(targetTooltipPosition.y, -canvasSize.y / 2 + tooltipSize.y / 2, canvasSize.y / 2 - tooltipSize.y / 2);

            tooltipRectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
        }
    }
}
