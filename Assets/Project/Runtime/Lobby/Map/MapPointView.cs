using System;
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
    }
}