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

        public event Action<MapPointView> OnClick;

        public MapPoint MapPoint { get; private set; }
        
        public void Link(MapPoint mapPoint)
        {
            MapPoint = mapPoint;
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
            if (!MapPoint.IsCanBeSelected) return;
            OnClick?.Invoke(this);
        }
    }
}