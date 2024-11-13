using System;
using DG.Tweening;
using Project.Runtime.Lobby.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.Lobby.Map
{
    public class MapPointView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image selectedImage;
        [SerializeField] private Image completedImage;
        [Space] 
        [SerializeField] private Image iconImage;
        [SerializeField] private Sprite bossIconSprite;
        [SerializeField] private Sprite bonusIconSprite;
        [SerializeField] private Sprite commonIconSprite;
        [Space]
        [SerializeField] private Image impactImage;

        public event Action<MapPointView> OnClick;

        public MapPoint MapPoint { get; private set; }
        
        public void Link(MapPoint mapPoint)
        {
            MapPoint = mapPoint;

            switch (mapPoint.PointType)
            {
                case MapPoint.MapPointType.Common:
                    iconImage.sprite = commonIconSprite;
                    iconImage.enabled = commonIconSprite;
                    break;
                case MapPoint.MapPointType.Bonus:
                    iconImage.sprite = bonusIconSprite;
                    iconImage.enabled = bonusIconSprite;
                    break;
                case MapPoint.MapPointType.Boss:
                    iconImage.sprite = bossIconSprite;
                    iconImage.enabled = bossIconSprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void SetCompleted(bool isCompleted)
        {
            completedImage.enabled = isCompleted;
        }

        public void SetSelected(bool value)
        {
            selectedImage.enabled = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(MapPoint.IsCanBeSelected);
            if (!MapPoint.IsCanBeSelected) return;
            OnClick?.Invoke(this);
        }

        public void Impact()
        {
            if (!MapPoint.IsCanBeSelected) return;
            impactImage.transform.DOKill(true);
            impactImage.transform.localScale = Vector3.one;
            impactImage.color = Color.white;
            DOTween.Sequence()
                .Append(impactImage.transform.DOScale(8, 1.25f).SetEase(Ease.Linear))
                .Join(impactImage.DOFade(0, 1.25f).SetEase(Ease.Linear))
                .SetLink(impactImage.gameObject)
                .SetDelay(0.5f);
        }
    }
}