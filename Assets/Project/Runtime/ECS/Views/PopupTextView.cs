using DG.Tweening;
using Project.Runtime.Features;
using TMPro;
using UnityEngine;

namespace Project.Runtime.ECS.Views
{
    public class PopupTextView : EntityView
    {
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private TextMeshProUGUI valueBlackText;

        private const float AnimationDuration = 1f; 
        
        public void SetValue(float value)
        {
            var formattedValue = value.FormatValue();
            valueText.SetText(formattedValue);
            valueBlackText.SetText(formattedValue);

            var endPosition = transform.position + new Vector3(Random.Range(-1, 1f), .75f, Random.Range(-1, 1f));
            transform.DOKill(true);
            DOTween.Sequence()
                .Append(transform.DOMove(endPosition, AnimationDuration).SetEase(Ease.OutQuart))
                .Join(transform.DOScale(Vector3.one * 1.1f, AnimationDuration).SetEase(Ease.Linear))
                .Join(transform.DOScale(Vector3.zero / 2f, AnimationDuration).SetEase(Ease.InQuart))
                .SetLink(gameObject);
        }
    }
}