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

            // Рассчитываем центр элемента
            Vector3 targetPosition = (corners[0] + corners[2]) / 2;

            // Переводим мировые координаты в экранные
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, targetPosition);

            // Переводим экранные координаты в локальные координаты Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPoint,
                uiCamera,
                out Vector2 localPoint);

            // Определяем размеры элемента
            Vector2 elementSize = targetRect.rect.size;

            // Начальная позиция тултипа рядом с элементом
            Vector2 tooltipPosition = localPoint;

            // Вычисляем базовые смещения
            float horizontalOffset = elementSize.x / 2.5f + tooltipRectTransform.rect.width / 4f;
            float verticalOffset = elementSize.y / 2f + tooltipRectTransform.rect.height / 2f;

            // Попытка разместить тултип в сторону центра экрана
            tooltipPosition.x += (localPoint.x > 0 ? -horizontalOffset : horizontalOffset);
            tooltipPosition.y += (localPoint.y > 0 ? -verticalOffset : verticalOffset);

            // Ограничиваем тултип в пределах Canvas
            RectTransform canvasRect = canvas.transform as RectTransform;
            Vector2 canvasSize = canvasRect.sizeDelta;
            Vector2 tooltipSize = tooltipRectTransform.sizeDelta;

            float clampedX = Mathf.Clamp(
                tooltipPosition.x,
                -canvasSize.x / 2 + tooltipSize.x / 2,
                canvasSize.x / 2 - tooltipSize.x / 2);

            float clampedY = Mathf.Clamp(
                tooltipPosition.y,
                -canvasSize.y / 2 + tooltipSize.y / 2,
                canvasSize.y / 2 - tooltipSize.y / 2);

            // Если тултип сдвинут за границы экрана, корректируем его положение
            if (tooltipPosition.x != clampedX)
            {
                tooltipPosition.x = clampedX;
                tooltipPosition.y = localPoint.y > 0 ? localPoint.y - verticalOffset : localPoint.y + verticalOffset;
            }

            if (tooltipPosition.y != clampedY)
            {
                tooltipPosition.y = clampedY;
                tooltipPosition.x =
                    localPoint.x > 0 ? localPoint.x - horizontalOffset : localPoint.x + horizontalOffset;
            }

            tooltipRectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
        }
    }
}