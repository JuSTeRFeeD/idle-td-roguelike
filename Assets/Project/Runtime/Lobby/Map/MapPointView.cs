using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Runtime.Lobby.Map
{
    public class MapPointView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image pointImage;
        [SerializeField] private Image selectedImage;
        [SerializeField] private Sprite completedPointSprite;
        [SerializeField] private Sprite defaultPointSprite;
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
            pointImage.sprite = isCompleted ? completedPointSprite : defaultPointSprite;
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